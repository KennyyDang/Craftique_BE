using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.UserVM
{
	public class ShipperViewModel : UserViewModel
	{
		public int PendingOrdersCount { get; set; }
	}
}
