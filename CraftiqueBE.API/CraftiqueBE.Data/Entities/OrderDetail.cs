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
	public class OrderDetail : IBaseEntity
	{
		[Key]
		public int OrderDetailID { get; set; }

		[ForeignKey("ProductItem")]
		public int? ProductItemID { get; set; }  // ➔ nullable vì có thể không có

		[ForeignKey("Order")]
		[Required(ErrorMessage = "Order ID is required.")]
		public int OrderID { get; set; }

		[Required(ErrorMessage = "Quantity is required.")]
		[Range(0, int.MaxValue, ErrorMessage = "Quantity must be at least 0.")]
		public int Quantity { get; set; }

		[Required(ErrorMessage = "Price is required.")]
		[Range(0, double.MaxValue, ErrorMessage = "Price must be at least 0.")]
		public double Price { get; set; }

		public bool IsDeleted { get; set; }

		public ProductItem ProductItem { get; set; }
		public Order Order { get; set; }
		public virtual ICollection<Review> Reviews { get; set; }

		// ➤ sửa lại quan hệ custom
		[ForeignKey("CustomProductFile")]
		public int? CustomProductFileID { get; set; }

		public CustomProductFile CustomProductFile { get; set; }
	}


}
