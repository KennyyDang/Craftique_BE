using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Helper;
using CraftiqueBE.Data.Models.ProductItemModel;
using CraftiqueBE.Data.ViewModels.ProductItemVM;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftiqueBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductItemController : ControllerBase
	{
		private readonly IProductItemServices _productItemServices;
		private readonly IMapper _mapper;

		public ProductItemController(IProductItemServices productItemServices, IMapper mapper)
		{
			_productItemServices = productItemServices;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<ActionResult<PagedResult<ProductItemViewModel>>> GetAll([FromQuery] ProductItemFilterModel filter)
		{
			try
			{
				var result = await _productItemServices.GetAllAsync(filter);

				var mappedResult = new PagedResult<ProductItemViewModel>
				{
					Items = _mapper.Map<List<ProductItemViewModel>>(result.Items),
					TotalItems = result.TotalItems,
					PageNumber = result.PageNumber,
					PageSize = result.PageSize,
					TotalPages = result.TotalPages
				};

				return Ok(mappedResult);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					new { message = ex.Message, details = ex.StackTrace });
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ProductItemViewModel>> GetById(int id)
		{
			var productItem = await _productItemServices.GetByIdAsync(id);
			return Ok(_mapper.Map<ProductItemViewModel>(productItem));
		}

		[HttpPost]
		[Authorize(Roles = $"{RolesHelper.Staff}, {RolesHelper.Admin}")]

		public async Task<ActionResult<ProductItemViewModel>> Add([FromBody] CreateProductItemModel createProductItem)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var productItem = _mapper.Map<ProductItem>(createProductItem);

			if (productItem == null)
			{
				return BadRequest("Invalid product item data.");
			}

			productItem = await _productItemServices.AddAsync(productItem);

			var result = _mapper.Map<ProductItemViewModel>(productItem);
			return CreatedAtAction(nameof(GetById),
				new { id = productItem.ProductItemID },
				result);
		}

		[HttpPut("{id}")]
		[Authorize(Roles = $"{RolesHelper.Staff}, {RolesHelper.Admin}")]

		public async Task<IActionResult> Update(int id, [FromBody] UpdateProductItemModel updateProductItem)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var productItem = await _productItemServices.UpdateAsync(id, updateProductItem);

			return Ok(_mapper.Map<ProductItemViewModel>(productItem));
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = $"{RolesHelper.Staff}, {RolesHelper.Admin}")]

		public async Task<IActionResult> Delete(int id)
		{
			var deletedProductItem = await _productItemServices.DeleteAsync(id);

			return Ok(_mapper.Map<ProductItemViewModel>(deletedProductItem));
		}

		[HttpGet("showall")]
		public async Task<ActionResult<IEnumerable<ProductItemViewModel>>> ShowAll()
		{
			try
			{
				var productItems = await _productItemServices.GetAllWithoutFilter();
				var mappedItems = _mapper.Map<List<ProductItemViewModel>>(productItems);
				return Ok(mappedItems);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					new { message = ex.Message });
			}
		}

		[HttpGet("{id}/total-sold")]
		public async Task<ActionResult> GetTotalSold(int id)
		{
			try
			{
				var result = await _productItemServices.GetTotalSoldForProductItemAsync(id);
				return Ok(result);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					new { message = ex.Message });
			}
		}
	}
}
