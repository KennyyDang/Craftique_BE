using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Helper;
using CraftiqueBE.Data.Models.ProductItemAttributeModel;
using CraftiqueBE.Data.ViewModels.ProductItemAttributeVM;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftiqueBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductItemAttributeController : ControllerBase
	{
		private readonly IProductItemAttributeServices _productItemAttributeServices;
		private readonly IMapper _mapper;

		public ProductItemAttributeController(IProductItemAttributeServices productItemAttributeServices, IMapper mapper)
		{
			_productItemAttributeServices = productItemAttributeServices;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<ProductItemAttributeViewModel>>> GetAll([FromQuery] ProductItemAttributeFilterModel filter)
		{
			try
			{
				if (filter == null)
				{
					filter = new ProductItemAttributeFilterModel();
				}

				var productItemAttributes = await _productItemAttributeServices.GetAllAsync(filter);
				return Ok(new
				{
					Items = _mapper.Map<List<ProductItemAttributeViewModel>>(productItemAttributes.Items),
					TotalItems = productItemAttributes.TotalItems,
					PageNumber = productItemAttributes.PageNumber,
					PageSize = productItemAttributes.PageSize,
					TotalPages = productItemAttributes.TotalPages
				});
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					new { message = ex.Message, details = ex.StackTrace });
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ProductItemAttributeViewModel>> GetById(int id)
		{
			var productItemAttribute = await _productItemAttributeServices.GetByIdAsync(id);
			return Ok(_mapper.Map<ProductItemAttributeViewModel>(productItemAttribute));
		}

		[HttpPost]
		[Authorize(Roles = $"{RolesHelper.Staff}, {RolesHelper.Admin}")]
		public async Task<ActionResult<ProductItemAttributeViewModel>> Add([FromBody] CreateProductItemAttributeModel createProductItemAttribute)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var productItemAttribute = _mapper.Map<ProductItemAttribute>(createProductItemAttribute);

			if (productItemAttribute == null)
			{
				return BadRequest("Invalid product item attribute data.");
			}

			productItemAttribute = await _productItemAttributeServices.AddAsync(productItemAttribute);

			return CreatedAtAction(nameof(GetById),
				new { id = productItemAttribute.ProductItemAttributeID },
				_mapper.Map<ProductItemAttributeViewModel>(productItemAttribute));
		}

		[HttpPut("{id}")]
		[Authorize(Roles = $"{RolesHelper.Staff}, {RolesHelper.Admin}")]

		public async Task<IActionResult> Update(int id, [FromBody] UpdateProductItemAttributeModel updateProductItemAttribute)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var productItemAttribute = await _productItemAttributeServices.UpdateAsync(id, updateProductItemAttribute);

			return Ok(_mapper.Map<ProductItemAttributeViewModel>(productItemAttribute));
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = $"{RolesHelper.Staff}, {RolesHelper.Admin}")]

		public async Task<IActionResult> Delete(int id)
		{
			var deletedProductItemAttribute = await _productItemAttributeServices.DeleteAsync(id);

			return Ok(_mapper.Map<ProductItemAttributeViewModel>(deletedProductItemAttribute));
		}
	}
}
