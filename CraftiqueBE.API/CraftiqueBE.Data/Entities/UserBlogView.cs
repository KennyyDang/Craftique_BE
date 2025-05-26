using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Entities
{
	public class UserBlogView
	{
		[Key]
		public int UserBlogViewID { get; set; }
		public string UserId { get; set; }
		public int BlogId { get; set; }
		public int Like { get; set; }


		[ForeignKey("BlogId")]
		public Blog Blog { get; set; }
		[ForeignKey("UserId")]
		public User User { get; set; }
	}
}
