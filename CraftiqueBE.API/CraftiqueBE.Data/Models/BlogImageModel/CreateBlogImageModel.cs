using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.BlogImageModel
{
	public class CreateBlogImageModel
	{
		[Required(ErrorMessage = "Image URL is required.")]
		public string ImageUrl { get; set; }
	}
}
