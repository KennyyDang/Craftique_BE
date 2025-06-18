using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.Models.PayOsModel;
using CraftiqueBE.Data.Models.PayOSModel;
using CraftiqueBE.Data.Models.WalletModel;
using CraftiqueBE.Data.ViewModels.WalletVM;
using CraftiqueBE.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace CraftiqueBE.Service.Services
{
	public class PaymentServices : IPaymentServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly PayOSConfig _payos;

		public PaymentServices(IUnitOfWork unitOfWork, IMapper mapper, IOptions<PayOSConfig> payosOptions)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_payos = payosOptions.Value;
		}

		public async Task<PaymentViewModel> CreatePaymentAsync(CreatePaymentModel model, string userId)
		{
			string orderId = Guid.NewGuid().ToString();
			string requestId = Guid.NewGuid().ToString();

			string rawData = $"{orderId}|{model.Amount}|{_payos.ReturnUrl}";
			string signature = CreateSHA256Signature(rawData, _payos.ChecksumKey);

			var payRequest = new PayOSRequestModel
			{
				orderCode = orderId,
				amount = model.Amount,
				description = "Nạp tiền vào ví người dùng",
				returnUrl = _payos.ReturnUrl,
				cancelUrl = _payos.CancelUrl,
				signature = signature
			};

			Console.WriteLine("PayOS Request: " + JsonSerializer.Serialize(payRequest));

			var client = new HttpClient
			{
				Timeout = TimeSpan.FromSeconds(30)
			};
			client.DefaultRequestHeaders.Add("x-client-id", _payos.ClientId);
			client.DefaultRequestHeaders.Add("x-api-key", _payos.ApiKey);

			var content = new StringContent(JsonSerializer.Serialize(payRequest), Encoding.UTF8, "application/json");
			int maxRetries = 3;
			for (int i = 0; i < maxRetries; i++)
			{
				try
				{
					var response = await client.PostAsync("https://sandbox-api.payos.vn/v2/payment-requests", content); // Sử dụng sandbox
					var body = await response.Content.ReadAsStringAsync();

					if (!response.IsSuccessStatusCode)
						throw new Exception($"PayOS failed: {body}");

					var result = JsonSerializer.Deserialize<PayOSResponseModel>(body);

					var payment = new Payment
					{
						UserId = userId,
						OrderId = orderId,
						RequestId = requestId,
						Amount = model.Amount,
						Status = "Pending",
						CreatedAt = DateTime.UtcNow,
						PayUrl = result.checkoutUrl,
						PayOSOrderCode = result.orderCode,
						Signature = signature,
						Provider = "PayOS"
					};

					await _unitOfWork.PaymentRepository.AddAsync(payment);
					await _unitOfWork.SaveChangesAsync();

					return _mapper.Map<PaymentViewModel>(payment);
				}
				catch (HttpRequestException ex)
				{
					Console.WriteLine($"Retry {i + 1} failed: {ex.Message}");
					if (i == maxRetries - 1) throw;
					await Task.Delay(1000 * (i + 1));
				}
			}
			throw new Exception("Không thể kết nối tới PayOS sau nhiều lần thử.");
		}

		public async Task<bool> UpdatePaymentStatusAsync(int paymentId, string newStatus)
		{
			var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(paymentId);
			if (payment == null) return false;

			payment.Status = newStatus;

			if (newStatus.ToLower() == "success")
			{
				payment.PaidAt = DateTime.UtcNow;

				var wallet = await EnsureWalletExistsAsync(payment.UserId);

				await _unitOfWork.WalletTransactionRepository.AddAsync(new WalletTransaction
				{
					WalletId = wallet.WalletId,
					Amount = payment.Amount,
					Description = "Nạp ví qua PayOS",
					Type = "Deposit",
					PaymentId = payment.PaymentId,
					CreatedAt = DateTime.UtcNow,
					IsDeleted = false
				});

				wallet.Balance += payment.Amount;
			}

			await _unitOfWork.SaveChangesAsync();
			return true;
		}

		public async Task<bool> UpdatePaymentStatusByOrderIdAsync(string orderId, string newStatus)
		{
			var payment = await _unitOfWork.PaymentRepository
				.GetAllQueryable()
				.FirstOrDefaultAsync(p => p.OrderId == orderId);

			if (payment == null) return false;

			return await UpdatePaymentStatusAsync(payment.PaymentId, newStatus);
		}

		private async Task<Wallet> EnsureWalletExistsAsync(string userId)
		{
			var wallet = await _unitOfWork.WalletRepository
				.GetAllQueryable()
				.FirstOrDefaultAsync(w => w.UserId == userId);

			if (wallet == null)
			{
				wallet = new Wallet
				{
					UserId = userId,
					Balance = 0,
					CreatedAt = DateTime.UtcNow,
					IsDeleted = false
				};

				await _unitOfWork.WalletRepository.AddAsync(wallet);
				await _unitOfWork.SaveChangesAsync();
			}

			return wallet;
		}

		private string CreateSHA256Signature(string data, string key)
		{
			using var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(key));
			var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
			return BitConverter.ToString(hash).Replace("-", "").ToLower();
		}
	}
}
