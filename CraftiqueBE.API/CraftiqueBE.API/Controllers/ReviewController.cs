using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Models.ProductItemModel;
using CraftiqueBE.Data.Models.ReviewModel;
using CraftiqueBE.Data.ViewModels.ReviewVM;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CraftiqueBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReviewController : ControllerBase
	{
		private readonly IReviewServices _reviewServices;
		private readonly IProductItemServices _productItemServices;
		private readonly IUserServices _userServices;
		private readonly IMapper _mapper;

		public ReviewController(IReviewServices reviewServices, IProductItemServices productItemServices,
			IUserServices userServices, IMapper mapper)
		{
			_reviewServices = reviewServices;
			_productItemServices = productItemServices;
			_userServices = userServices;
			_mapper = mapper;
		}

		[HttpPost]
		public async Task<ActionResult<ReviewViewModel>> Add([FromBody] CreateReviewModel createReview)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var review = _mapper.Map<Review>(createReview);

			if (review == null)
				return BadRequest("Invalid review data.");

			review = await _reviewServices.AddAsync(review);

			return Ok(_mapper.Map<ReviewViewModel>(review));
		}

		[HttpPost("product")]
		public async Task<ActionResult<ReviewViewModel>> AddProductReview([FromBody] CreateProductReviewModel createReview)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var review = new Review
			{
				UserID = createReview.UserID,
				OrderDetailID = createReview.OrderDetailID,
				ProductItemID = createReview.ProductItemID,
				ProductRating = createReview.Rating,
				ProductComment = createReview.Comment
			};

			review = await _reviewServices.AddAsync(review);

			return Ok(_mapper.Map<ReviewViewModel>(review));
		}

		[HttpPost("shipper")]
		public async Task<ActionResult<ReviewViewModel>> AddShipperReview([FromBody] CreateShipperReviewModel createReview)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var review = new Review
			{
				UserID = createReview.UserID,
				OrderDetailID = createReview.OrderDetailID,
				ProductItemID = createReview.ProductItemID,
				ShipperID = createReview.ShipperID,
				ShipperRating = createReview.Rating,
				ShipperComment = createReview.Comment
			};

			review = await _reviewServices.AddAsync(review);

			return Ok(_mapper.Map<ReviewViewModel>(review));
		}

		[HttpGet("product/{productItemId}/details")]
		public async Task<ActionResult<ProductRatingViewModel>> GetProductDetails(int productItemId)
		{
			var productItem = await _productItemServices.GetByIdAsync(productItemId);
			if (productItem == null)
				return NotFound($"Product item with ID {productItemId} not found.");

			var reviews = await _reviewServices.GetByProductItemIdAsync(productItemId);

			var (averageRating, totalReviewers) = await _reviewServices.GetProductItemRatingStatisticsAsync(productItemId);

			return Ok(new ProductRatingViewModel
			{
				ProductItemID = productItemId,
				ProductName = productItem.Name,
				AverageRating = averageRating,
				TotalReviewers = totalReviewers,
				Reviews = _mapper.Map<List<ReviewViewModel>>(reviews)
			});
		}

		[HttpGet("shipper/{shipperId}/details")]
		public async Task<ActionResult<ShipperRatingViewModel>> GetShipperDetails(string shipperId)
		{
			var shipper = await _userServices.GetByIdAsync(shipperId);
			if (shipper == null)
				return NotFound($"Shipper with ID {shipperId} not found.");

			var reviews = await _reviewServices.GetAllAsync(r => r.ShipperID == shipperId && !r.IsDeleted);

			var (averageRating, totalReviewers) = await _reviewServices.GetShipperRatingStatisticsAsync(shipperId);

			return Ok(new ShipperRatingViewModel
			{
				ShipperID = shipperId,
				ShipperName = shipper.Name,
				AverageRating = averageRating,
				TotalReviewers = totalReviewers,
				Reviews = _mapper.Map<List<ReviewViewModel>>(reviews)
			});
		}

		[HttpGet("products/summary")]
		public async Task<ActionResult<IEnumerable<ProductRatingViewModel>>> GetAllProductRatings()
		{
			var filter = new ProductItemFilterModel
			{
				PageNumber = 1,
				PageSize = 1000
			};

			var productItemsResult = await _productItemServices.GetAllAsync(filter);
			var productItems = productItemsResult.Items;

			var allRatings = await _reviewServices.GetAllProductRatingsAsync();
			var allReviews = await _reviewServices.GetAllProductReviewsAsync();
			var reviewsByProduct = allReviews.GroupBy(r => r.ProductItemID).ToDictionary(g => g.Key, g => g.ToList());

			var result = new List<ProductRatingViewModel>();

			foreach (var product in productItems)
			{
				double avgRating = 0;
				int totalReviewers = 0;
				List<ReviewViewModel> productReviews = new List<ReviewViewModel>();

				if (allRatings.TryGetValue(product.ProductItemID, out var stats))
				{
					avgRating = stats.AverageRating;
					totalReviewers = stats.TotalReviewers;
				}

				if (reviewsByProduct.TryGetValue(product.ProductItemID, out var reviews))
				{
					productReviews = _mapper.Map<List<ReviewViewModel>>(reviews);
				}

				result.Add(new ProductRatingViewModel
				{
					ProductItemID = product.ProductItemID,
					ProductName = product.Name,
					AverageRating = avgRating,
					TotalReviewers = totalReviewers,
					Reviews = productReviews
				});
			}

			return Ok(result);
		}

		[HttpGet("shippers/summary")]
		public async Task<ActionResult<IEnumerable<ShipperRatingViewModel>>> GetAllShipperRatings()
		{
			var shippers = await _userServices.GetShippersAsync();

			var allRatings = await _reviewServices.GetAllShipperRatingsAsync();

			var result = new List<ShipperRatingViewModel>();

			foreach (var shipper in shippers)
			{
				double avgRating = 0;
				int totalReviewers = 0;

				if (allRatings.TryGetValue(shipper.Id, out var stats))
				{
					avgRating = stats.AverageRating;
					totalReviewers = stats.TotalReviewers;
				}

				result.Add(new ShipperRatingViewModel
				{
					ShipperID = shipper.Id,
					ShipperName = shipper.Name,
					AverageRating = avgRating,
					TotalReviewers = totalReviewers
				});
			}

			return Ok(result);
		}

		[HttpGet("user/{userId}/product-rating-status")]
		public async Task<ActionResult<Dictionary<int, bool>>> GetUserProductRatingStatus(
			string userId,
			[FromQuery] List<int> orderDetailIds)
		{
			if (orderDetailIds == null || !orderDetailIds.Any())
				return BadRequest("Order detail IDs are required.");

			var user = await _userServices.GetByIdAsync(userId);
			if (user == null)
				return NotFound($"User with ID {userId} not found.");

			var ratingStatus = await _reviewServices.GetUserProductRatingStatusAsync(userId, orderDetailIds);

			return Ok(ratingStatus);
		}

		[HttpGet("user/{userId}/shipper/{shipperId}/order/{orderId}/has-rated")]
		public async Task<ActionResult<bool>> HasUserRatedShipper(string userId, string shipperId, int orderId)
		{
			var user = await _userServices.GetByIdAsync(userId);
			if (user == null)
				return NotFound($"User with ID {userId} not found.");

			var shipper = await _userServices.GetByIdAsync(shipperId);
			if (shipper == null)
				return NotFound($"Shipper with ID {shipperId} not found.");

			var hasRated = await _reviewServices.HasUserRatedShipperAsync(userId, shipperId, orderId);

			return Ok(hasRated);
		}
	}
}
