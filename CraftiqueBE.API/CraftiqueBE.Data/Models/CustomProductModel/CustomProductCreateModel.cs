using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.CustomProductModel
{
	public class CustomProductCreateModel
	{
		public int ProductID { get; set; }
		public string? CustomName { get; set; }
		public string? Description { get; set; }
		public decimal Price { get; set; }
		//public string UserId { get; set; }
	}
}
