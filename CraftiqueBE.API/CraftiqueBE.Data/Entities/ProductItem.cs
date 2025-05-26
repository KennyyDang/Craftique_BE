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
	public class ProductItem : IBaseEntity
	{
		[Key]
		public int ProductItemID { get; set; }

		[ForeignKey("Product")]
		[Required(ErrorMessage = "Product ID is required.")]
		public int ProductID { get; set; }

		[Required(ErrorMessage = "Product item name is required.")]
		[MaxLength(255, ErrorMessage = "Product item name cannot exceed 255 characters.")]
		public string Name { get; set; }

		[MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
		public string? Description { get; set; }

		[Required(ErrorMessage = "Quantity is required.")]
		[Range(0, int.MaxValue, ErrorMessage = "Quantity must be at least 0.")]
		public int Quantity { get; set; }

		public int DisplayIndex { get; set; }

		public double Price { get; set; }

		public bool IsDeleted { get; set; }

		public virtual Product Product { get; set; }

		public virtual ICollection<ProductImg> ProductImgs { get; set; }
		public virtual ICollection<Review> Reviews { get; set; }
		public virtual ICollection<OrderDetail> OrderDetails { get; set; }
		public virtual ICollection<ProductItemAttribute> ProductItemAttributes { get; set; }
		public virtual ICollection<Voucher> Vouchers { get; set; }
	}
}
