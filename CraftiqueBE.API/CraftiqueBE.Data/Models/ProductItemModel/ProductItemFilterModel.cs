using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.ProductItemModel
{
	public class ProductItemFilterModel
	{
		public string? SearchTerm { get; set; }
		public decimal? MinPrice { get; set; }
		public decimal? MaxPrice { get; set; }
		public string? PriceSort { get; set; }

		// Các thuộc tính đặc trưng cho sản phẩm gốm
		public List<string>? Colors { get; set; }
		public List<string>? Materials { get; set; } // ví dụ: đất nung, sứ, gốm tráng men
		public List<string>? Sizes { get; set; }     // ví dụ: nhỏ, vừa, lớn
		public List<string>? Shapes { get; set; }    // ví dụ: tròn, vuông, bầu dục

		public int? CategoryId { get; set; }

		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 8;

		public void ValidatePageNumber(int totalPages)
		{
			if (PageNumber < 1)
				PageNumber = 1;
			if (totalPages > 0 && PageNumber > totalPages)
				PageNumber = totalPages;
		}
	}

	public class PagedResult<T>
	{
		public List<T> Items { get; set; }
		public int TotalItems { get; set; }
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public int TotalPages { get; set; }

		public int? NextPage => HasNextPage ? PageNumber + 1 : null;
		public int? PreviousPage => HasPreviousPage ? PageNumber - 1 : null;
		public bool HasNextPage => PageNumber < TotalPages;
		public bool HasPreviousPage => PageNumber > 1;

		public bool IsFirstPage => PageNumber == 1;
		public bool IsLastPage => PageNumber == TotalPages;

		public IEnumerable<int> Pages
		{
			get
			{
				const int maxPages = 5;
				var halfMax = maxPages / 2;

				var start = Math.Max(1, PageNumber - halfMax);
				var end = Math.Min(TotalPages, start + maxPages - 1);

				if (end == TotalPages)
				{
					start = Math.Max(1, end - maxPages + 1);
				}

				return Enumerable.Range(start, Math.Min(maxPages, end - start + 1));
			}
		}
	}
}
