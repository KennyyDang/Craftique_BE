using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.CustomProductModel
{
	public class CustomProductUploadModel
	{
		public int ProductID { get; set; }

		public string? CustomName { get; set; }

		public string? Description { get; set; }

		public decimal Price { get; set; }

		public IFormFile? Image { get; set; }  // ➤ Thêm file ảnh
	}
}
