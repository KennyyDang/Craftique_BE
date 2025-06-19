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
	public class PaymentTransaction : IBaseEntity
	{
		[Key]
		public int TransactionId { get; set; }

		[ForeignKey("Payment")]
		public int PaymentId { get; set; }

		public string Type { get; set; } = "PayOS"; // Hoặc Momo, ZaloPay sau này

		public string Status { get; set; } // Success, Failed, Pending

		public decimal Amount { get; set; }

		public string? Description { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public virtual Payment Payment { get; set; }
		public bool IsDeleted { get; set; } = false;
	}
}
