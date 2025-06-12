using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.ViewModels.WalletVM;
using CraftiqueBE.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Services
{
	public class WalletServices : IWalletServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public WalletServices(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<WalletViewModel> GetWalletByUserIdAsync(string userId)
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

			return _mapper.Map<WalletViewModel>(wallet);
		}

		public async Task<bool> RechargeWalletAsync(string userId, decimal amount, string description = null)
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

			wallet.Balance += amount;

			await _unitOfWork.WalletTransactionRepository.AddAsync(new WalletTransaction
			{
				WalletId = wallet.WalletId,
				Amount = amount,
				Description = description ?? "Recharge",
				Type = "Deposit",
				CreatedAt = DateTime.UtcNow,
				IsDeleted = false
			});

			await _unitOfWork.SaveChangesAsync();
			return true;
		}

		public async Task<IEnumerable<WalletTransactionViewModel>> GetWalletTransactionsAsync(string userId)
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

			var transactions = await _unitOfWork.WalletTransactionRepository
				.GetAllQueryable()
				.Where(t => t.WalletId == wallet.WalletId)
				.OrderByDescending(t => t.CreatedAt)
				.ToListAsync();

			return _mapper.Map<IEnumerable<WalletTransactionViewModel>>(transactions);
		}
	}
}
