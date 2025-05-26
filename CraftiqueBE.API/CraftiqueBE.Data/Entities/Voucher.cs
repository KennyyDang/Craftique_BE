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
	public class Voucher : IBaseEntity
	{
		[Key]
		public int VoucherID { get; set; }

		[Required(ErrorMessage = "Voucher name is required.")]
		[MaxLength(255, ErrorMessage = "Voucher name cannot exceed 255 characters.")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Voucher code is required.")]
		[MaxLength(255, ErrorMessage = "Voucher code cannot exceed 255 characters.")]
		public string Code { get; set; }

		[Required(ErrorMessage = "Discount type is required.")]
		[MaxLength(50, ErrorMessage = "Discount type cannot exceed 50 characters.")]
		public string DiscountType { get; set; }

		[Range(0, int.MaxValue, ErrorMessage = "Discount value must be a positive number.")]
		public int DiscountValue { get; set; }

		[Range(0.01, double.MaxValue, ErrorMessage = "Minimum total must be greater than 0.")]
		public double MinimumTotal { get; set; }

		public DateTime CreatedDate { get; set; }

		[Compare("CreatedDate", ErrorMessage = "Expiration date must be later than the created date.")]
		public DateTime ExpiredDate { get; set; }

		public bool IsDeleted { get; set; }


		[ForeignKey("ProductItem")]
		public int ProductItemID { get; set; }

		[ForeignKey("Product")]
		public int ProductID { get; set; }

		public virtual ProductItem ProductItem { get; set; }
		public virtual Product Product { get; set; }
		public virtual ICollection<Order> Orders { get; set; }
	}
}
