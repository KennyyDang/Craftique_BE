using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Services
{
	public class ReviewServices : IReviewServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public ReviewServices(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<Review> AddAsync(Review review)
		{
			await _unitOfWork.BeginTransactionAsync();
			try
			{
				var user = await _unitOfWork.UserRepository.GetSingleByConditionAsynce(u => u.Id == review.UserID);
				if (user == null)
					throw new KeyNotFoundException($"User with ID {review.UserID} not found.");

				var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(review.ProductItemID);
				if (productItem == null)
					throw new KeyNotFoundException($"ProductItem with ID {review.ProductItemID} not found.");

				var orderDetail = await _unitOfWork.OrderDetailRepository.GetByIdAsync(review.OrderDetailID);
				if (orderDetail == null)
					throw new KeyNotFoundException($"OrderDetail with ID {review.OrderDetailID} not found.");

				var existingProductReview = await _unitOfWork.ReviewRepository.GetSingleByConditionAsynce(
					r => r.OrderDetailID == review.OrderDetailID &&
						 r.ProductItemID == review.ProductItemID &&
						 r.UserID == review.UserID &&
						 !r.IsDeleted);

				if (existingProductReview != null && review.ProductRating.HasValue)
				{
					existingProductReview.ProductRating = review.ProductRating;
					existingProductReview.ProductComment = review.ProductComment;
					existingProductReview.Date = DateTime.Now;
					bool updateResult = await _unitOfWork.ReviewRepository.Update(existingProductReview);
					await _unitOfWork.SaveChangesAsync();
					await _unitOfWork.CommitTransactionAsync();
					return existingProductReview;
				}

				if (review.ShipperID != null && review.ShipperRating.HasValue)
				{
					var shipper = await _unitOfWork.UserRepository.GetSingleByConditionAsynce(u => u.Id == review.ShipperID);
					if (shipper == null)
						throw new KeyNotFoundException($"Shipper with ID {review.ShipperID} not found.");

					int orderId = orderDetail.OrderID;

					var orderDetails = await _unitOfWork.OrderDetailRepository.GetAllAsync(od => od.OrderID == orderId);

					if (orderDetails == null || !orderDetails.Any())
						throw new KeyNotFoundException($"Order details not found for order ID {orderId}");

					var orderDetailIds = orderDetails.Select(od => od.OrderDetailID);
					var existingShipperReviews = await _unitOfWork.ReviewRepository.GetAllAsync(
						r => orderDetailIds.Contains(r.OrderDetailID) &&
							 r.ShipperID == review.ShipperID &&
							 r.UserID == review.UserID &&
							 r.ShipperRating.HasValue &&
							 !r.IsDeleted);

					if (existingShipperReviews != null && existingShipperReviews.Any())
					{
						var latestShipperReview = existingShipperReviews.OrderByDescending(r => r.Date).FirstOrDefault();
						latestShipperReview.ShipperRating = review.ShipperRating;
						latestShipperReview.ShipperComment = review.ShipperComment;
						latestShipperReview.Date = DateTime.Now;

						bool updateResult = await _unitOfWork.ReviewRepository.Update(latestShipperReview);
						await _unitOfWork.SaveChangesAsync();
						await _unitOfWork.CommitTransactionAsync();

						return latestShipperReview;
					}

					if (review.ProductRating.HasValue)
					{
						// 
					}
					else
					{
						var shipperOnlyReview = new Review
						{
							UserID = review.UserID,
							OrderDetailID = orderDetails.First().OrderDetailID,
							ProductItemID = orderDetails.First().ProductItemID,
							ShipperID = review.ShipperID,
							ShipperRating = review.ShipperRating,
							ShipperComment = review.ShipperComment,
							Date = DateTime.Now
						};

						var result = await _unitOfWork.ReviewRepository.AddAsync(shipperOnlyReview);
						await _unitOfWork.SaveChangesAsync();
						await _unitOfWork.CommitTransactionAsync();

						return result;
					}
				}

				review.Date = DateTime.Now;
				var newReview = await _unitOfWork.ReviewRepository.AddAsync(review);

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return newReview;
			}
			catch
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}

		public async Task<IEnumerable<Review>> GetAllAsync(Expression<Func<Review, bool>> predicate)
		{
			var reviews = await _unitOfWork.ReviewRepository.GetAllAsync(
				predicate,
				r => r.User,
				r => r.Shipper,
				r => r.ProductItem
			);

			return reviews?.OrderByDescending(r => r.Date).ToList() ?? new List<Review>();
		}

		public async Task<IEnumerable<Review>> GetByProductItemIdAsync(int productItemId)
		{
			var reviews = await _unitOfWork.ReviewRepository.GetAllAsync(
				r => r.ProductItemID == productItemId && !r.IsDeleted,
				r => r.User,
				r => r.Shipper
			);

			return reviews?.OrderByDescending(r => r.Date).ToList() ?? new List<Review>();
		}

		public async Task<IEnumerable<Review>> GetByUserIdAsync(string userId)
		{
			var reviews = await _unitOfWork.ReviewRepository.GetAllAsync(
				r => r.UserID == userId && !r.IsDeleted,
				r => r.ProductItem,
				r => r.Shipper
			);

			return reviews?.OrderByDescending(r => r.Date).ToList() ?? new List<Review>();
		}

		public async Task<(double AverageRating, int TotalReviewers)> GetProductItemRatingStatisticsAsync(int productItemId)
		{
			var reviews = await _unitOfWork.ReviewRepository.GetAllAsync(
				r => r.ProductItemID == productItemId && !r.IsDeleted && r.ProductRating.HasValue
			);

			if (reviews == null || !reviews.Any())
				return (0, 0);

			var reviewsList = reviews.ToList();
			double averageRating = reviewsList.Average(r => r.ProductRating.Value);
			int totalReviewers = reviewsList.Count;

			return (Math.Round(averageRating, 1), totalReviewers);
		}

		public async Task<(double AverageRating, int TotalReviewers)> GetShipperRatingStatisticsAsync(string shipperId)
		{
			var reviews = await _unitOfWork.ReviewRepository.GetAllAsync(
				r => r.ShipperID == shipperId && !r.IsDeleted && r.ShipperRating.HasValue
			);

			if (reviews == null || !reviews.Any())
				return (0, 0);

			var reviewsList = reviews.ToList();
			double averageRating = reviewsList.Average(r => r.ShipperRating.Value);
			int totalReviewers = reviewsList.Count;

			return (Math.Round(averageRating, 1), totalReviewers);
		}

		public async Task<IEnumerable<Review>> GetAllProductReviewsAsync()
		{
			var reviews = await _unitOfWork.ReviewRepository.GetAllAsync(
				r => !r.IsDeleted && r.ProductRating.HasValue,
				r => r.User,
				r => r.ProductItem,
				r => r.Shipper
			);

			return reviews?.OrderByDescending(r => r.Date).ToList() ?? new List<Review>();
		}

		public async Task<IEnumerable<Review>> GetAllShipperReviewsAsync()
		{
			var reviews = await _unitOfWork.ReviewRepository.GetAllAsync(
				r => !r.IsDeleted && r.ShipperRating.HasValue && r.ShipperID != null,
				r => r.User,
				r => r.Shipper,
				r => r.ProductItem
			);

			return reviews?.OrderByDescending(r => r.Date).ToList() ?? new List<Review>();
		}

		public async Task<IDictionary<int, (double AverageRating, int TotalReviewers)>> GetAllProductRatingsAsync()
		{
			var reviews = await _unitOfWork.ReviewRepository.GetAllAsync(
				r => !r.IsDeleted && r.ProductRating.HasValue,
				r => r.ProductItem
			);

			if (reviews == null || !reviews.Any())
				return new Dictionary<int, (double, int)>();

			var result = reviews
				.GroupBy(r => r.ProductItemID)
				.ToDictionary(
					g => g.Key,
					g => (
						AverageRating: Math.Round(g.Average(r => r.ProductRating.Value), 1),
						TotalReviewers: g.Count()
					)
				);

			return result;
		}

		public async Task<IDictionary<string, (double AverageRating, int TotalReviewers)>> GetAllShipperRatingsAsync()
		{
			var reviews = await _unitOfWork.ReviewRepository.GetAllAsync(
				r => !r.IsDeleted && r.ShipperRating.HasValue && r.ShipperID != null,
				r => r.Shipper
			);

			if (reviews == null || !reviews.Any())
				return new Dictionary<string, (double, int)>();

			var result = reviews
				.GroupBy(r => r.ShipperID)
				.ToDictionary(
					g => g.Key,
					g => (
						AverageRating: Math.Round(g.Average(r => r.ShipperRating.Value), 1),
						TotalReviewers: g.Count()
					)
				);

			return result;
		}

		public async Task<bool> HasUserRatedProductAsync(string userId, int productItemId, int orderDetailId)
		{
			var review = await _unitOfWork.ReviewRepository.GetSingleByConditionAsynce(
				r => r.UserID == userId &&
					r.ProductItemID == productItemId &&
					r.OrderDetailID == orderDetailId &&
					r.ProductRating.HasValue &&
					!r.IsDeleted);

			return review != null;
		}

		public async Task<bool> HasUserRatedShipperAsync(string userId, string shipperId, int orderId)
		{
			var orderDetails = await _unitOfWork.OrderDetailRepository.GetAllAsync(od => od.OrderID == orderId);

			if (orderDetails == null || !orderDetails.Any())
				return false;

			var orderDetailIds = orderDetails.Select(od => od.OrderDetailID);

			var review = await _unitOfWork.ReviewRepository.GetSingleByConditionAsynce(
				r => r.UserID == userId &&
					r.ShipperID == shipperId &&
					orderDetailIds.Contains(r.OrderDetailID) &&
					r.ShipperRating.HasValue &&
					!r.IsDeleted);

			return review != null;
		}

		public async Task<Dictionary<int, bool>> GetUserProductRatingStatusAsync(string userId, IEnumerable<int> orderDetailIds)
		{
			if (orderDetailIds == null || !orderDetailIds.Any())
				return new Dictionary<int, bool>();

			var reviews = await _unitOfWork.ReviewRepository.GetAllAsync(
				r => r.UserID == userId &&
					orderDetailIds.Contains(r.OrderDetailID) &&
					r.ProductRating.HasValue &&
					!r.IsDeleted);

			var result = orderDetailIds.ToDictionary(
				id => id,
				id => reviews.Any(r => r.OrderDetailID == id)
			);

			return result;
		}
	}
}
