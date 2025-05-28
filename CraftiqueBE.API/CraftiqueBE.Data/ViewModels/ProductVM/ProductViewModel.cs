using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.ProductVM
{
	public class ProductViewModel
	{
		public int ProductID { get; set; }
		public int CategoryID { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }
		public bool IsDeleted { get; set; }
		public int DisplayIndex { get; set; }
		public string CategoryName { get; set; }
	}
}
