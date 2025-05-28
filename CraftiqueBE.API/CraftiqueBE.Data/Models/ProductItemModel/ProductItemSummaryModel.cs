using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.ProductItemModel
{
	public class ProductItemSummaryModel
	{
		public int CategoryId { get; set; }
		public int ProductId { get; set; }
		public int TotalQuantity { get; set; }
	}
}
