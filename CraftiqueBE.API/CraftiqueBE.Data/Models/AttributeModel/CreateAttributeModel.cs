using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.AttributeModel
{
	public class CreateAttributeModel
	{
		[Required(ErrorMessage = "Attribute name is required.")]
		[MaxLength(255, ErrorMessage = "Attribute name cannot exceed 255 characters.")]
		public string? AttributeName { get; set; }

		[MaxLength(1000, ErrorMessage = "Data Type cannot exceed 1000 characters.")]
		public string? DataType { get; set; }

		public int CategoryID { get; set; }
	}
}
