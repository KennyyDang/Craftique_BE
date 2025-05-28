using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.ProductItemModel
{
	public class CreateProductItemModel
	{
		[Required(ErrorMessage = "Product ID is required.")]
		public int ProductID { get; set; }

		[Required(ErrorMessage = "Name is required.")]
		[MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters.")]
		public string Name { get; set; }

		[MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
		public string? Description { get; set; }

		[Required(ErrorMessage = "Quantity is required.")]
		public int Quantity { get; set; }

		public int DisplayIndex { get; set; }

		[Required(ErrorMessage = "Price is required.")]
		[Range(0, double.MaxValue, ErrorMessage = "Price must be at least 0.")]
		public double Price { get; set; }

		public List<string> ImageUrls { get; set; } = new List<string>();

		public bool IsDeleted = false;
	}
}
