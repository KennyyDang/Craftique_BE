using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.DesignUploadModel
{
	public class CreateCustomizationModel
	{
		public int ProductItemID { get; set; }
		public int? UserDesignUploadID { get; set; } // ảnh khách tải
		public string? CustomText { get; set; }
		public string? FontFamily { get; set; }
		public string? Color { get; set; }
	}
}
