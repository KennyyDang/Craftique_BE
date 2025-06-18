using CraftiqueBE.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Entities
{
	public class WalletTransaction : IBaseEntity
	{
		[Key]
		public int TransactionId { get; set; }

		[ForeignKey("Wallet")]
		public int WalletId { get; set; }

		[Required]
		public string Type { get; set; } // Deposit, Withdraw, Purchase, Refund

		[Required]
		public decimal Amount { get; set; }

		public string Description { get; set; }

		/// <summary>
		/// Tham chiếu đến bảng thanh toán nếu đây là giao dịch qua PayOS/MoMo
		/// </summary>
		public int? PaymentId { get; set; } // Nullable để cho phép transaction không cần payment

		[ForeignKey("PaymentId")]
		public virtual Payment? Payment { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public bool IsDeleted { get; set; }

		public virtual Wallet Wallet { get; set; }
	}
}
