using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.ProductItemAttributeModel
{
	public class ProductItemAttributeFilterModel
	{
		public string? SearchAttributeName { get; set; }
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 10;

		public void ValidatePageNumber(int totalPages)
		{
			if (PageNumber < 1)
				PageNumber = 1;
			if (totalPages > 0 && PageNumber > totalPages)
				PageNumber = totalPages;
		}
	}
}
