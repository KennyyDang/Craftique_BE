using CraftiqueBE.Data.ViewModels.ProductImgVM;
using CraftiqueBE.Data.ViewModels.ProductItemAttributeVM;
using CraftiqueBE.Data.ViewModels.ProductVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.ProductItemVM
{
	public class ProductItemViewModel
	{
		public int ProductItemID { get; set; }

		public int ProductID { get; set; }

		public string Name { get; set; }

		public string? Description { get; set; }

		public int Quantity { get; set; }

		public int DisplayIndex { get; set; }

		public double Price { get; set; }

		public bool IsDeleted { get; set; }

		public ProductViewModel Product { get; set; }
		public ICollection<ProductImgViewModel> ProductImgs { get; set; }
		public ICollection<ProductItemAttributeViewModel> ProductItemAttributes { get; set; }
	}
}
