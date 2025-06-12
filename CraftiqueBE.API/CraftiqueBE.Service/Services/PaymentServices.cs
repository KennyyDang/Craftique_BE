using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.Models.WalletModel;
using CraftiqueBE.Data.ViewModels.WalletVM;
using CraftiqueBE.Service.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace CraftiqueBE.Service.Services
{
	public class PaymentServices : IPaymentServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public PaymentServices(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<PaymentViewModel> CreatePaymentAsync(CreatePaymentModel model)
		{
			var payment = _mapper.Map<Payment>(model);
			payment.CreatedAt = DateTime.UtcNow;
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

				var wallet = await _unitOfWork.WalletRepository
					.GetAllQueryable()
					.FirstOrDefaultAsync(w => w.UserId == payment.UserId);

				if (wallet == null)
					throw new Exception("Wallet not found for the user.");

				await _unitOfWork.WalletTransactionRepository.AddAsync(new WalletTransaction
				{
					WalletId = wallet.WalletId,
					Amount = payment.Amount,
					Description = "MoMo Top-up",
					Type = "Deposit",
					CreatedAt = DateTime.UtcNow,
					IsDeleted = false
				});

				wallet.Balance += payment.Amount;
			}

			await _unitOfWork.SaveChangesAsync();
			return true;
		}

	}
}
