using AutoMapper;
using CraftiqueBE.Data.Helper;
using CraftiqueBE.Data.Models.ProductImgModel;
using CraftiqueBE.Data.ViewModels.ProductImgVM;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftiqueBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductImgsController : ControllerBase
	{
		private readonly IProductImgServices _productImgServices;
		private readonly IMapper _mapper;

		public ProductImgsController(IProductImgServices productImgServices, IMapper mapper)
		{
			_productImgServices = productImgServices;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<ProductImgViewModel>>> GetAll()
		{
			try
			{
				var productImgs = await _productImgServices.GetAllAsync();
				return Ok(_mapper.Map<List<ProductImgViewModel>>(productImgs));
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					new { message = ex.Message, details = ex.StackTrace });
			}
		}

		[HttpPost]
		[Authorize(Roles = $"{RolesHelper.Staff}, {RolesHelper.Admin}")]
		public async Task<IActionResult> AddMultipleImages([FromBody] CreateProductImgModel model)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				var result = await _productImgServices.AddMultipleAsync(model);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					statusCode = 500,
					message = ex.Message,
					errorType = ex.GetType().Name,
					stackTrace = ex.StackTrace
				});
			}
		}
	}
}
