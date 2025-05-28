using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.ReviewModel
{
	public class CreateShipperReviewModel
	{
		[Required(ErrorMessage = "User ID is required.")]
		public string UserID { get; set; }

		[Required(ErrorMessage = "Order Detail ID is required.")]
		public int OrderDetailID { get; set; }

		[Required(ErrorMessage = "Product Item ID is required.")]
		public int ProductItemID { get; set; }

		[Required(ErrorMessage = "Shipper ID is required.")]
		public string ShipperID { get; set; }

		[Required(ErrorMessage = "Rating is required.")]
		[Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
		public int Rating { get; set; }

		[MaxLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
		public string? Comment { get; set; }
	}
}
