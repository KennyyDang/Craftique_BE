using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Entities
{
	public class CustomProduct : IBaseEntity
	{
		[Key]
		public int CustomProductID { get; set; }

		[Required]
		public int ProductID { get; set; } // Sản phẩm gốc
		[MaxLength(1000)]
		public string? ImageUrl { get; set; }
		[MaxLength(255)]
		public string? CustomName { get; set; } // Tên custom, có thể để trống
		[MaxLength(1000)]
		public string? Description { get; set; } // Mô tả yêu cầu custom
		public decimal Price { get; set; }
		public bool IsDeleted { get; set; }

		public virtual ICollection<CustomProductFile> CustomProductFiles { get; set; }
	}
}
