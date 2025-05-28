using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.ProductModel
{
	public class CreateProductModel
	{
		[Required(ErrorMessage = "Product ID is required.")]
		public int ProductID { get; set; }

		[Required(ErrorMessage = "Category ID is required.")]
		public int CategoryID { get; set; }

		[Required(ErrorMessage = "Product name is required.")]
		[MaxLength(255, ErrorMessage = "Product name cannot exceed 255 characters.")]
		public string Name { get; set; }

		[MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
		public string? Description { get; set; }

		public int DisplayIndex { get; set; }

		public bool IsDeleted = false;
	}
}
