using AutoMapper;
using CraftiqueBE.Data.Models.AttributeModel;
using CraftiqueBE.Data.ViewModels.AttributeVM;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CraftiqueBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AttributeController : ControllerBase
	{
		private readonly IAttributeServices _attributeServices;
		private readonly IMapper _mapper;

		public AttributeController(IAttributeServices attributeServices, IMapper mapper)
		{
			_attributeServices = attributeServices;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<AttributeViewModel>>> GetAll()
		{
			try
			{
				var attributes = await _attributeServices.GetAllAsync();
				return Ok(_mapper.Map<List<AttributeViewModel>>(attributes));
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					new { message = ex.Message, details = ex.StackTrace });
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<AttributeViewModel>> GetById(int id)
		{
			var attribute = await _attributeServices.GetByIdAsync(id);
			return Ok(_mapper.Map<AttributeViewModel>(attribute));
		}

		[HttpPost]
		public async Task<ActionResult<Data.Entities.Attribute>> Add([FromBody] CreateAttributeModel createAttribute)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var attribute = _mapper.Map<Data.Entities.Attribute>(createAttribute);

			if (attribute == null)
			{
				return BadRequest("Invalid attribute data.");
			}

			attribute = await _attributeServices.AddAsync(attribute);

			return CreatedAtAction(nameof(GetById),
				new { id = attribute.AttributeID },
				_mapper.Map<AttributeViewModel>(attribute));
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] UpdateAttributeModel updateAttribute)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var attribute = await _attributeServices.UpdateAsync(id, updateAttribute);

			return Ok(_mapper.Map<AttributeViewModel>(attribute));
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAttribute(int id)
		{
			var deletedAttribute = await _attributeServices.DeleteAsync(id);

			return Ok(_mapper.Map<AttributeViewModel>(deletedAttribute));
		}
	}
}
