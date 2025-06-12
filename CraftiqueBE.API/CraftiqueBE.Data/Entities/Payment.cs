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
		public string OrderId { get; set; }

		[Required]
		public string RequestId { get; set; }

		public string MomoOrderId { get; set; }

		public string Signature { get; set; }

		public string Status { get; set; } // Pending, Success, Failed

		public decimal Amount { get; set; }

		public string PayUrl { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public DateTime? PaidAt { get; set; }

		public bool IsDeleted { get; set; }

		public virtual User User { get; set; }
	}

}
