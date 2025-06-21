using CraftiqueBE.Data.Helper;
using CraftiqueBE.Data.Models.EmailModel;
using CraftiqueBE.Data.Models.WorkshopRegistrationModel;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftiqueBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WorkshopController : ControllerBase
	{
		private readonly IWorkshopServices _workshopService;

		public WorkshopController(IWorkshopServices workshopService)
		{
			_workshopService = workshopService;
		}

		// ✅ GET: /api/workshop/registrations - Lấy danh sách người đăng ký
		[HttpGet("registrations")]
		[Authorize(Roles = $"{RolesHelper.Admin}, {RolesHelper.Staff}")]
		public async Task<IActionResult> GetAll()
		{
			var result = await _workshopService.GetAllAsync();
			return Ok(result);
		}

		// ✅ POST: /api/workshop/register - Người dùng đăng ký workshop
		[HttpPost("register")]
		[Authorize(Roles = RolesHelper.Customer)]
		public async Task<IActionResult> Register([FromBody] CreateWorkshopRegistrationModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _workshopService.AddAsync(model);
			return Ok(result);
		}

		// ✅ POST: /api/workshop/send-email - Gửi email cho 1 người đăng ký
		[HttpPost("send-email")]
		[Authorize(Roles = $"{RolesHelper.Admin}, {RolesHelper.Staff}")]
		public async Task<IActionResult> SendEmail([FromBody] SendWorkshopEmailModel model)
		{
			await _workshopService.SendEmailAsync(model.RegistrationId, model.Subject, model.Body);
			return Ok(new { message = "Đã gửi email thành công." });
		}

		// ✅ POST: /api/workshop/send-email-bulk - Gửi email hàng loạt
		[HttpPost("send-email-bulk")]
		[Authorize(Roles = $"{RolesHelper.Admin}, {RolesHelper.Staff}")]
		public async Task<IActionResult> SendEmailBulk([FromBody] SendWorkshopBulkEmailModel model)
		{
			await _workshopService.SendEmailBulkAsync(model.RegistrationIds, model.Subject, model.Body);
			return Ok(new { message = "Đã gửi email hàng loạt thành công." });
		}

		// ✅ POST: /api/workshop/confirm/{id} - Admin xác nhận người đăng ký
		[HttpPost("confirm/{id}")]
		[Authorize(Roles = $"{RolesHelper.Admin}, {RolesHelper.Staff}")]
		public async Task<IActionResult> Confirm(int id)
		{
			var success = await _workshopService.ConfirmAsync(id);
			if (success)
				return Ok(new { message = "Xác nhận thành công và đã gửi email thông báo." });

			return BadRequest("Xác nhận thất bại.");
		}
	}
}
