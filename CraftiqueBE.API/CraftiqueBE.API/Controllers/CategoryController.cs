using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Helper;
using CraftiqueBE.Data.Models.CategoryModel;
using CraftiqueBE.Data.ViewModels.CategoryVM;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftiqueBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CategoryController : ControllerBase
	{
		private readonly ICategoryServices _categoryServices;
		private readonly IMapper _mapper;

		public CategoryController(ICategoryServices categoryServices, IMapper mapper)
		{
			_categoryServices = categoryServices;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<CategoryViewModel>>> GetAll(string? categoryName = null)
		{
			try
			{
				var categories = await _categoryServices.GetAllAsync(categoryName);
				return Ok(_mapper.Map<List<CategoryViewModel>>(categories));
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message, details = ex.StackTrace });
			}
		}


		[HttpGet("{id}")]
		public async Task<ActionResult<CategoryViewModel>> GetById(int id)

		{
			var category = await _categoryServices.GetByIdAsync(id);
			return Ok(_mapper.Map<CategoryViewModel>(category));

		}



		[HttpPost()]
		[Authorize(Roles = $"{RolesHelper.Staff}, {RolesHelper.Admin}")]
		public async Task<ActionResult<Category>> Add([FromBody] CreateCategoryModel Createcategory)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var category = _mapper.Map<Category>(Createcategory);

			if (category == null)

			{
				return BadRequest("Invalid category data.");
			}

			category = await _categoryServices.AddAsync(category);

			return CreatedAtAction(nameof(GetById), new { id = category.CategoryID }, _mapper.Map<CategoryViewModel>(category));
		}

		[HttpPut("{id}")]
		[Authorize(Roles = $"{RolesHelper.Staff}, {RolesHelper.Admin}")]
		public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryModel updateCategory)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var category = await _categoryServices.UpdateAsync(id, updateCategory);

			return Ok(_mapper.Map<CategoryViewModel>(category));
		}


		[HttpDelete("{id}")]
		[Authorize(Roles = $"{RolesHelper.Staff}, {RolesHelper.Admin}")]
		public async Task<IActionResult> DeleteCategory(int id)
		{
			var deletedCategory = await _categoryServices.DeleteAsync(id);

			return Ok(_mapper.Map<CategoryViewModel>(deletedCategory));
		}

	}
}
