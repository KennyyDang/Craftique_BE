using CraftiqueBE.Data.Models.AdminModel;
using CraftiqueBE.Data.Models.ProductItemModel;
using CraftiqueBE.Data.ViewModels.UserVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Interfaces
{
	public interface IAdminServices
	{
		Task<object> GetTopCustomersAsync(TimeModel model, int? topN);
		Task<object> GetTopSellingProductItemsAsync(TimeModel model, int? topN);
		Task<object> GetTotalProductItemsAsync(CategoryProductFilterModel filter);
		Task<object> GetTotalRevenueAsync(TimeModel model);
		Task<int> GetTotalUserAsync();
		Task<IEnumerable<ShipperViewModel>> GetAllShippersWithPendingOrdersAsync();
	}
}
