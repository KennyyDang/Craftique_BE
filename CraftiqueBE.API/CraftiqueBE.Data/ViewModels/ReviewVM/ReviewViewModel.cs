using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.ReviewVM
{
	public class ReviewViewModel
	{
		public int ReviewID { get; set; }
		public string UserID { get; set; }
		public int OrderDetailID { get; set; }
		public int ProductItemID { get; set; }
		public string? ShipperID { get; set; }
		public DateTime Date { get; set; }
		public int? ProductRating { get; set; }
		public int? ShipperRating { get; set; }
		public string? ProductComment { get; set; }
		public string? ShipperComment { get; set; }
		public string UserName { get; set; }
		public string? ShipperName { get; set; }
	}

	public class RatingStatisticsViewModel
	{
		public double AverageRating { get; set; }
		public int TotalReviewers { get; set; }
	}

	public class ProductRatingViewModel
	{
		public int ProductItemID { get; set; }
		public string ProductName { get; set; }
		public double AverageRating { get; set; }
		public int TotalReviewers { get; set; }
		public List<ReviewViewModel> Reviews { get; set; } = new List<ReviewViewModel>();
	}

	public class ShipperRatingViewModel
	{
		public string ShipperID { get; set; }
		public string ShipperName { get; set; }
		public double AverageRating { get; set; }
		public int TotalReviewers { get; set; }
		public List<ReviewViewModel> Reviews { get; set; } = new List<ReviewViewModel>();
	}
}
