using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.BlogVM
{
	public class BlogViewModel
	{
		public int BlogID { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public string Author { get; set; }
		public int ProductId { get; set; }
		public DateTime UploadDate { get; set; }
		public DateTime UpdateDate { get; set; }
		public int View { get; set; }
		public int Like { get; set; }
		public bool IsDeleted { get; set; }
		public List<BlogImageViewModel> BlogImages { get; set; } = new List<BlogImageViewModel>();
	}
	public class BlogImageViewModel
	{
		public int BlogImageID { get; set; }
		public string ImageUrl { get; set; }
		public int BlogId { get; set; }
	}
}
