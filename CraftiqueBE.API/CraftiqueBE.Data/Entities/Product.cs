using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.Entities;

namespace CraftiqueBE.Data.Models
{
	public class Product : IBaseEntity
	{
		[Key]
		public int ProductID { get; set; }

		[ForeignKey("Category")]
		[Required(ErrorMessage = "Category ID is required.")]
		public int CategoryID { get; set; }

		[Required(ErrorMessage = "Product name is required.")]
		[MaxLength(255, ErrorMessage = "Product name cannot exceed 255 characters.")]
		public string Name { get; set; }

		[MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
		public string? Description { get; set; }

		public bool IsDeleted { get; set; }

		public int DisplayIndex { get; set; }

		public virtual Category Category { get; set; }

		public virtual ICollection<ProductItem> ProductItems { get; set; }
		public virtual ICollection<Blog> Blogs { get; set; }
		public virtual ICollection<Voucher> Vouchers { get; set; }
		public virtual ICollection<CustomProduct> CustomProducts { get; set; }
	}
}
