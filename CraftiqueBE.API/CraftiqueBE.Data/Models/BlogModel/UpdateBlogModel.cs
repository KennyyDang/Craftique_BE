using CraftiqueBE.Data.Models.BlogImageModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.BlogModel
{
	public class UpdateBlogModel
	{
		[Required(ErrorMessage = "Title is required.")]
		public string Title { get; set; }

		[Required(ErrorMessage = "Content is required.")]
		public string Content { get; set; }

		public string Author { get; set; }
		public int ProductId { get; set; }

		public List<CreateBlogImageModel> BlogImages { get; set; } = new List<CreateBlogImageModel>();
	}
}
