using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Entities
{
	public class Category : IBaseEntity
	{
		[Key]
		public int CategoryID { get; set; }

		[Required(ErrorMessage = "Category name is required.")]
		[MaxLength(255, ErrorMessage = "Category name cannot exceed 255 characters.")]
		public string Name { get; set; }

		[MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
		public string? Description { get; set; }

		public int DisplayIndex { get; set; }

		public bool IsDeleted { get; set; }

		public virtual ICollection<Product> Products { get; set; }
	}
}
