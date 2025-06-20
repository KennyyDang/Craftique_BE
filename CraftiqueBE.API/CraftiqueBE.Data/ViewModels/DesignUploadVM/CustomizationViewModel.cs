using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.UserDesignUploadVM
{
	public class CustomizationViewModel
	{
		public int ID { get; set; }
		public int ProductItemID { get; set; }
		public string? FileUrl { get; set; }
		public string? CustomText { get; set; }
		public string? FontFamily { get; set; }
		public string? Color { get; set; }
		public string? PreviewImageUrl { get; set; }
	}

}
