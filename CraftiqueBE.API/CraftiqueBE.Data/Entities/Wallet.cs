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
	public class Wallet : IBaseEntity
	{
		[Key]
		public int WalletId { get; set; }

		[ForeignKey("User")]
		public string UserId { get; set; }

		public decimal Balance { get; set; } = 0;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public bool IsDeleted { get; set; }

		public virtual User User { get; set; }

		public virtual ICollection<WalletTransaction> WalletTransactions { get; set; }
	}

}
