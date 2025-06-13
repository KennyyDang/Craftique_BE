using CraftiqueBE.Data.Helper;
using CraftiqueBE.Data.Models.MomoModel;
using CraftiqueBE.Data.Models.WalletModel;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CraftiqueBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WalletController : ControllerBase
	{
		private readonly IWalletServices _walletService;
		private readonly IPaymentServices _paymentService;

		public WalletController(IWalletServices walletService, IPaymentServices paymentService)
		{
			_walletService = walletService;
			_paymentService = paymentService;
		}

		/// <summary>
		/// Lấy thông tin ví của người dùng đang đăng nhập
		/// </summary>
		[HttpGet("my-wallet")]
		[Authorize(Roles = $"{RolesHelper.Customer}, {RolesHelper.Staff}, {RolesHelper.Admin}")]
		public async Task<IActionResult> GetMyWallet()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null) return Unauthorized("Missing user info");

			var wallet = await _walletService.GetWalletByUserIdAsync(userId);
			return Ok(wallet);
		}

		/// <summary>
		/// Lấy lịch sử giao dịch của ví
		/// </summary>
		[HttpGet("my-transactions")]
		[Authorize(Roles = $"{RolesHelper.Customer}, {RolesHelper.Staff}, {RolesHelper.Admin}")]
		public async Task<IActionResult> GetMyTransactions()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null) return Unauthorized("Missing user info");

			var transactions = await _walletService.GetWalletTransactionsAsync(userId);
			return Ok(transactions);
		}

		/// <summary>
		/// Tạo yêu cầu nạp tiền vào ví (qua Momo)
		/// </summary>
		[HttpPost("request-topup")]
		[Authorize(Roles = RolesHelper.Customer)]
		public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentModel model)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			model.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var payment = await _paymentService.CreatePaymentAsync(model);

			return Ok(payment); // Trong đó sẽ trả về PayUrl (link redirect)
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
		/// Callback từ MoMo gọi về khi thanh toán thành công hoặc thất bại
		/// </summary>
		[HttpPost("momo/callback")]
		[AllowAnonymous] // MoMo sẽ không có token, nên phải cho phép gọi
		public async Task<IActionResult> MomoCallback([FromBody] MomoCallbackModel callback)
		{
			if (callback.ResultCode == 0)
			{
				var success = await _paymentService.UpdatePaymentStatusByOrderIdAsync(callback.OrderID, "Success");
				return Ok(new { status = "confirmed", success });
			}
			else
			{
				await _paymentService.UpdatePaymentStatusByOrderIdAsync(callback.OrderID, "Failed");
				return BadRequest(new { status = "failed", message = callback.Message });
			}
		}
	}
}
