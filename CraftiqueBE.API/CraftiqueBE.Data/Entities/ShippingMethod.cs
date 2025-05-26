using CraftiqueBE.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Entities
{
	public class ShippingMethod : IBaseEntity
	{
		[Key]
		public int ShippingMethodID { get; set; }

		[MaxLength(250, ErrorMessage = "Shipping method name cannot exceed 250 characters.")]
		public string Name { get; set; }

		[Range(0, double.MaxValue, ErrorMessage = "Shipping price must be at least 0.")]
		public float ShippingPrice { get; set; }
		public bool IsDeleted { get; set; }
		public ICollection<Order> Orders { get; set; }
	}
}
