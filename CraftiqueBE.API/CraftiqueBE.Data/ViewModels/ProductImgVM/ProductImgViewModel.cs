using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.ProductImgVM
{
	public class ProductImgViewModel
	{
		public int ProductImgID { get; set; }
		public int ProductItemID { get; set; }
		public string ImageUrl { get; set; }
		public bool IsDeleted { get; set; }
	}
}
