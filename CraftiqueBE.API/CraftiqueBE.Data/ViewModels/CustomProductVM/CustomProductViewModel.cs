using CraftiqueBE.Data.ViewModels.CustomProductVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.CustomProductVM
{
	public class CustomProductViewModel
	{
		public int CustomProductID { get; set; }
		public int ProductID { get; set; }
		public string? CustomName { get; set; }
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public string UserId { get; set; }

		public List<CustomProductFileViewModel> Files { get; set; }
	}
}
