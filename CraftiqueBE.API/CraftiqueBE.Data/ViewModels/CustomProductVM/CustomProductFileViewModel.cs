using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.CustomProductVM
{
	public class CustomProductFileViewModel
	{
		public int CustomProductFileID { get; set; }
		public string? FileUrl { get; set; }
		public string? CustomText { get; set; }
		public DateTime UploadedAt { get; set; }
		public int Quantity { get; set; }
		public string? CustomProductImageUrl { get; set; }
		public string? CustomProductName { get; set; }
	}
}
