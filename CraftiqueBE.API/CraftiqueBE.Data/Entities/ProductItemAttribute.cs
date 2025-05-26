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
	public class ProductItemAttribute : IBaseEntity
	{
		[Key]
		public int ProductItemAttributeID { get; set; }

		public int ProductItemID { get; set; }

		public int AttributeID { get; set; }

		public string Value { get; set; }

		public bool IsDeleted { get; set; }

		[ForeignKey("ProductItemID")]
		public virtual ProductItem ProductItem { get; set; }

		[ForeignKey("AttributeID")]
		public virtual Attribute Attribute { get; set; }
	}
}
