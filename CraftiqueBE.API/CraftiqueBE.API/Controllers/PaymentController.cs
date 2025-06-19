using CraftiqueBE.Data.Helper;
using CraftiqueBE.Data.Models.WalletModel;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace CraftiqueBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly IPaymentServices _paymentService;

		public PaymentController(IPaymentServices paymentService)
		{
			_paymentService = paymentService;
		}

		/// <summary>
		/// Lấy danh sách giao dịch thanh toán của người dùng hiện tại
		/// </summary>
		[HttpGet("transactions")]
		[Authorize(Roles = RolesHelper.Customer)]
		public async Task<IActionResult> GetMyTransactions()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized("Không tìm thấy thông tin người dùng");
			}

			// Gọi dịch vụ để lấy danh sách giao dịch
			var transactions = await _paymentService.GetTransactionsByUserIdAsync(userId);

			return Ok(transactions); // Trả về danh sách TransactionViewModel
		}


		/// <summary>
		/// Tạo yêu cầu nạp tiền vào ví qua PayOS
		/// </summary>
		[HttpPost("request-topup")]
		[Authorize(Roles = RolesHelper.Customer)]
		public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentModel model)
		{
			// Lấy UserId từ token xác thực
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized("Không tìm thấy thông tin người dùng. Vui lòng đăng nhập lại.");
			}
			// Kiểm tra ModelState
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Gọi dịch vụ để tạo payment
			var payment = await _paymentService.CreatePaymentAsync(model, userId);

			return Ok(new { payUrl = payment.PayUrl }); // Trả về PayUrl
		}

		/// <summary>
		/// Dành cho Admin cập nhật trạng thái payment thủ công (nếu cần)
		/// </summary>
		[HttpPut("payment-status/{paymentId}")]
		[Authorize(Roles = RolesHelper.Admin)]
		public async Task<IActionResult> UpdatePaymentStatus(int paymentId, [FromQuery] string newStatus)
		{
			var result = await _paymentService.UpdatePaymentStatusAsync(paymentId, newStatus);
			return result ? Ok("Payment updated") : NotFound("Payment not found");
		}

		/// <summary>
		/// Callback từ PayOS khi thanh toán thành công
		/// </summary>
		[HttpPost("payos/webhook")]
		[AllowAnonymous] // PayOS không dùng token
		public async Task<IActionResult> PayOSCallback([FromBody] JsonElement payload)
		{
			var orderCode = payload.GetProperty("orderCode").GetString();
			var status = payload.GetProperty("status").GetString(); // "PAID" hoặc "CANCELLED"

			if (string.IsNullOrEmpty(orderCode)) return BadRequest("Missing order code");

			if (status?.ToUpper() == "PAID")
			{
				var success = await _paymentService.UpdatePaymentStatusByOrderIdAsync(orderCode, "Success");
				return Ok(new { message = "Thanh toán thành công", success });
			}
			else
			{
				await _paymentService.UpdatePaymentStatusByOrderIdAsync(orderCode, "Failed");
				return BadRequest(new { message = "Thanh toán thất bại", status });
			}
		}
	}
}
