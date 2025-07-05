using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.OrderVM
{
	public class OrderStatisticsViewModel
	{
		public string[] Labels { get; set; } // Tên tháng (VD: "Jan 2025", "Feb 2025")
		public int[] OrderCounts { get; set; } // Số lượng đơn hàng
		public double[] TotalAmounts { get; set; } // Tổng số tiền
	}
}
