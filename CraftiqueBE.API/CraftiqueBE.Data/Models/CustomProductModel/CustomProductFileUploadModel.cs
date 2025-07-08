using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.CustomProductModel
{
	public class CustomProductFileUploadModel
	{
		public int CustomProductID { get; set; }   
		public IFormFile? File { get; set; }       
		public string? CustomText { get; set; }
		[Required(ErrorMessage = "Quantity is required.")]
		[Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
		public int Quantity { get; set; }
	}
}
