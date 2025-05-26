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
	public class Order : IBaseEntity
	{
		[Key]
		public int OrderID { get; set; }

		[ForeignKey("User")]
		[Required(ErrorMessage = "User ID is required.")]
		public string UserID { get; set; }

		[ForeignKey("Shipper")]
		public string? ShipperID { get; set; }

		[Required(ErrorMessage = "Order date is required.")]
		public DateTime OrderDate { get; set; }

		[MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
		public string Address { get; set; }

		[MaxLength(500, ErrorMessage = "Payment method cannot exceed 500 characters.")]
		public string PaymentMethod { get; set; }

		[ForeignKey("ShippingMethod")]
		[Required(ErrorMessage = "Shipping method is required.")]
		public int ShippingMethodID { get; set; }

		[Required(ErrorMessage = "Total amount is required.")]
		[Range(0, double.MaxValue, ErrorMessage = "Total must be a positive value.")]
		public double Total { get; set; }

		[MaxLength(250, ErrorMessage = "Order status cannot exceed 250 characters.")]
		public string OrderStatus { get; set; }

		[ForeignKey("Voucher")]
		public int? VoucherID { get; set; }
		public bool IsDeleted { get; set; }

		public virtual User User { get; set; }
		public virtual User Shipper { get; set; }
		public virtual ShippingMethod ShippingMethod { get; set; }
		public virtual Voucher Voucher { get; set; }
		public virtual ICollection<OrderDetail> OrderDetails { get; set; }
	}
}
