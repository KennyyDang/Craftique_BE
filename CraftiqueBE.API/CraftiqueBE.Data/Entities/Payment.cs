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
	public class Payment : IBaseEntity
	{
		[Key]
		public int PaymentId { get; set; }

		[ForeignKey("User")]
		public string UserId { get; set; }

		[Required]
		public string OrderId { get; set; } // Hoặc mã nạp nếu là nạp ví

		[Required]
		public string RequestId { get; set; } // Dùng làm định danh duy nhất giao dịch nội bộ

		public string? MomoOrderId { get; set; }

		public string? PayOSOrderCode { get; set; } // mã đơn của PayOS
		public string? PayOSCheckoutUrl { get; set; }

		public string? Signature { get; set; } // dùng để xác thực

		public string Status { get; set; } // Pending, Success, Failed

		public decimal Amount { get; set; }

		public string PayUrl { get; set; } // Tổng hợp URL redirect

		public string Provider { get; set; } = "PayOS"; // hoặc "MoMo", "ZaloPay"

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? PaidAt { get; set; }

		public bool IsDeleted { get; set; }

		public virtual User User { get; set; }
	}


}
