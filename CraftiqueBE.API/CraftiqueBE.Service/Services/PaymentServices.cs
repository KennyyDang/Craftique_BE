using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.Models.PayOsModel;
using CraftiqueBE.Data.Models.WalletModel;
using CraftiqueBE.Data.ViewModels.PaymentVM;
using CraftiqueBE.Data.ViewModels.WalletVM;
using CraftiqueBE.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using System.Text.Json;

namespace CraftiqueBE.Service.Services
{
	public class PaymentServices : IPaymentServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly PayOSConfig _payos;
		private readonly PayOS _payosSdk;

		public PaymentServices(IUnitOfWork unitOfWork, IMapper mapper, IOptions<PayOSConfig> payosOptions)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_payos = payosOptions.Value;
			_payosSdk = new PayOS(_payos.ClientId, _payos.ApiKey, _payos.ChecksumKey);
		}

		public async Task<PaymentViewModel> CreatePaymentAsync(CreatePaymentModel model, string userId)
		{
			var orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // PayOS yêu cầu dạng số
			var description = "Thanh toán đơn hàng";

			var items = new List<ItemData>
			{
				new ItemData("Sản phẩm Craftique", 1, (int)(model.Amount)) // giá phải * 1000
            };

			var paymentData = new PaymentData(
				orderCode: orderCode,
				amount: (int)(model.Amount),
				description: description,
				items: items,
				returnUrl: _payos.ReturnUrl,
				cancelUrl: _payos.CancelUrl
			);
			Console.WriteLine("Payment Data: " + JsonSerializer.Serialize(paymentData));
			var result = await _payosSdk.createPaymentLink(paymentData);
			Console.WriteLine("PayOS Result: " + result.checkoutUrl);

			var payment = new Payment
			{
				UserId = userId,
				OrderId = orderCode.ToString(),
				RequestId = Guid.NewGuid().ToString(),
				Amount = model.Amount,
				Status = "Pending",
				PayUrl = result.checkoutUrl,
				PayOSOrderCode = orderCode.ToString(),
				Provider = "PayOS",
				CreatedAt = DateTime.UtcNow
			};

			await _unitOfWork.PaymentRepository.AddAsync(payment);
			await _unitOfWork.SaveChangesAsync();

			return _mapper.Map<PaymentViewModel>(payment);
		}

		public async Task<bool> UpdatePaymentStatusAsync(int paymentId, string newStatus)
		{
			var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(paymentId);
			if (payment == null) return false;

			payment.Status = newStatus;

			if (newStatus.ToLower() == "success")
			{
				payment.PaidAt = DateTime.UtcNow;

				var transaction = new PaymentTransaction
				{
					PaymentId = payment.PaymentId,
					Type = payment.Provider, // "PayOS" hoặc các provider khác nếu có
					Status = "Success",
					Amount = payment.Amount,
					Description = "Thanh toán thành công qua " + payment.Provider,
					CreatedAt = DateTime.UtcNow
				};

				await _unitOfWork.TransactionRepository.AddAsync(transaction);
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

		public async Task<List<TransactionViewModel>> GetTransactionsByUserIdAsync(string userId)
		{
			var transactions = await _unitOfWork.TransactionRepository
				.GetAllAsync(t => t.Payment.UserId == userId, t => t.Payment);

			var result = _mapper.Map<List<TransactionViewModel>>(transactions.OrderByDescending(t => t.CreatedAt).ToList());
			return result;
		}

	}
}
