using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.AttributeVM
{
	public class AttributeViewModel
	{
		public int AttributeID { get; set; }
		public string? AttributeName { get; set; }
		public string? DataType { get; set; }
		public int CategoryID { get; set; }
	}
}
