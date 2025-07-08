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
	public class CustomProductFile : IBaseEntity
	{
		[Key]
		public int CustomProductFileID { get; set; }

		[Required]
		public int CustomProductID { get; set; }
		[Required]
		public string UserId { get; set; }

		[MaxLength(1000)]
		public string? FileUrl { get; set; } // KHÔNG REQUIRED

		[MaxLength(255)]
		public string? FileName { get; set; } // KHÔNG REQUIRED

		[MaxLength(1000)]
		public string? CustomText { get; set; } // KHÔNG REQUIRED

		public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
		public int Quantity { get; set; }

		public bool IsDeleted { get; set; }

		[ForeignKey("CustomProductID")]
		public virtual CustomProduct CustomProduct { get; set; }
	}

}
