using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.ProductItemAttributeVM
{
	public class ProductItemAttributeViewModel
	{
		public int ProductItemAttributeID { get; set; }
		public int ProductItemID { get; set; }
		public int AttributeID { get; set; }
		public string Value { get; set; }
		public bool IsDeleted { get; set; }
		public string AttributeName { get; set; }
	}
}
