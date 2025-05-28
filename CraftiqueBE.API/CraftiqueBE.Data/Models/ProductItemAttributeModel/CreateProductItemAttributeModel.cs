using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.ProductItemAttributeModel
{
	public class CreateProductItemAttributeModel
	{
		[Required(ErrorMessage = "ProductItem ID is required.")]
		public int ProductItemID { get; set; }

		[Required(ErrorMessage = "Attribute ID is required.")]
		public int AttributeID { get; set; }

		[Required(ErrorMessage = "Value is required.")]
		[MaxLength(255, ErrorMessage = "Value cannot exceed 255 characters.")]
		public string Value { get; set; }
	}
}
