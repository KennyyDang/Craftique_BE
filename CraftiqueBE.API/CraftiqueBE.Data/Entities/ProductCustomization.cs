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
	public class ProductCustomization : IBaseEntity
	{
		[Key]
		public int ProductCustomizationID { get; set; }

		[Required]
		public int ProductItemID { get; set; }

		public int? UserDesignUploadID { get; set; } // Có thể null nếu chỉ có text

		[MaxLength(1000)]
		public string? CustomText { get; set; }

		[MaxLength(255)]
		public string? FontFamily { get; set; }

		[MaxLength(100)]
		public string? Color { get; set; }

		[MaxLength(1000)]
		public string? PreviewImageUrl { get; set; } // mockup nếu có render

		public bool IsDeleted { get; set; }

		// Navigation
		[ForeignKey("ProductItemID")]
		public virtual ProductItem ProductItem { get; set; }

		[ForeignKey("UserDesignUploadID")]
		public virtual UserDesignUpload UserDesignUpload { get; set; }
	}
}
