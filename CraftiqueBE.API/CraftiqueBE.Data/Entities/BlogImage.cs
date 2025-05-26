using CraftiqueBE.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Entities
{
	public class BlogImage : IBaseEntity
	{
		[Key]
		public int BlogImageID { get; set; }
		public string ImageUrl { get; set; }
		public bool IsDeleted { get; set; } = false;

		[ForeignKey("BlogId")]
		public int BlogId { get; set; }

		public Blog Blog { get; set; }
	}
}
