using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Helper;
using CraftiqueBE.Data.Models.BlogModel;
using CraftiqueBE.Data.ViewModels.BlogVM;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftiqueBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BlogController : ControllerBase
	{
		private readonly IBlogServices _blogServices;
		private readonly IMapper _mapper;

		public BlogController(IBlogServices blogServices, IMapper mapper)
		{
			_blogServices = blogServices;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<BlogViewModel>>> GetAll([FromQuery] string search = null)
		{
			try
			{
				var blogs = await _blogServices.GetAllAsync(search);
				return Ok(_mapper.Map<List<BlogViewModel>>(blogs));
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					new { message = ex.Message, details = ex.StackTrace });
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<BlogViewModel>> GetById(int id)
		{
			var blog = await _blogServices.GetByIdAsync(id);
			return Ok(_mapper.Map<BlogViewModel>(blog));
		}

		[HttpPost]
		[Authorize(Roles = $"{RolesHelper.Staff}, {RolesHelper.Admin}")]
		public async Task<ActionResult<BlogViewModel>> Add([FromBody] CreateBlogModel createBlog)
		{
			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage);
				return BadRequest(new { errors });
			}

			var blog = _mapper.Map<Blog>(createBlog);


			if (blog == null)
				return BadRequest("Invalid blog data.");

			blog = await _blogServices.AddAsync(blog);

			return CreatedAtAction(nameof(GetById),
				new { id = blog.BlogID },
				_mapper.Map<BlogViewModel>(blog));
		}

		[HttpPut("{id}")]
		[Authorize(Roles = $"{RolesHelper.Staff}, {RolesHelper.Admin}")]
		public async Task<IActionResult> Update(int id, [FromBody] UpdateBlogModel updateBlog)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var blog = await _blogServices.UpdateAsync(id, updateBlog);
			return Ok(_mapper.Map<BlogViewModel>(blog));
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = $"{RolesHelper.Staff}, {RolesHelper.Admin}")]
		public async Task<IActionResult> Delete(int id)
		{
			var deletedBlog = await _blogServices.DeleteAsync(id);
			return Ok(_mapper.Map<BlogViewModel>(deletedBlog));
		}
	}
}
