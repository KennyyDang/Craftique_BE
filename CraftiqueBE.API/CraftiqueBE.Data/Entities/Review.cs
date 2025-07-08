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
	public class Review : IBaseEntity
	{
		public int ReviewID { get; set; }

		[ForeignKey("User")]
		[Required(ErrorMessage = "User ID is required.")]
		public string UserID { get; set; }

		[ForeignKey("OrderDetail")]
		[Required(ErrorMessage = "Order Detail ID is required.")]
		public int OrderDetailID { get; set; }

		[ForeignKey("ProductItem")]
		[Required(ErrorMessage = "Product Item ID is required.")]
		public int? ProductItemID { get; set; }

		[ForeignKey("Shipper")]
		public string? ShipperID { get; set; }
		public DateTime Date { get; set; }

		[Range(1, 5, ErrorMessage = "Product rating must be between 1 and 5.")]
		public int? ProductRating { get; set; }

		[Range(1, 5, ErrorMessage = "Shipper rating must be between 1 and 5.")]
		public int? ShipperRating { get; set; }

		[MaxLength(1000, ErrorMessage = "Product comment cannot exceed 1000 characters.")]
		public string? ProductComment { get; set; }

		[MaxLength(1000, ErrorMessage = "Shipper comment cannot exceed 1000 characters.")]
		public string? ShipperComment { get; set; }

		public bool IsDeleted { get; set; }
		public User User { get; set; }
		public User Shipper { get; set; }
		public OrderDetail OrderDetail { get; set; }
		public ProductItem ProductItem { get; set; }
	}
}
