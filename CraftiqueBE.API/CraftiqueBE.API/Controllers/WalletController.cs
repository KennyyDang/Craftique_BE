// WalletController.cs
using CraftiqueBE.Data.Helper;
using CraftiqueBE.Data.Models.WalletModel;
using CraftiqueBE.Service.Interfaces;
using CraftiqueBE.Service.Services;
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

		[HttpGet("my-wallet")]
		[Authorize(Roles = $"{RolesHelper.Customer}, {RolesHelper.Staff}, {RolesHelper.Admin}")]
		public async Task<IActionResult> GetMyWallet()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null) return Unauthorized("Missing user info.");

			var wallet = await _walletService.GetWalletByUserIdAsync(userId);
			return Ok(wallet);
		}

		[HttpGet("my-transactions")]
		[Authorize(Roles = $"{RolesHelper.Customer}, {RolesHelper.Staff}, {RolesHelper.Admin}")]
		public async Task<IActionResult> GetMyTransactions()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null) return Unauthorized("Missing user info.");

			var transactions = await _walletService.GetWalletTransactionsAsync(userId);
			return Ok(transactions);
		}

		[HttpPost("request-topup")]
		[Authorize(Roles = RolesHelper.Customer)]
		public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentModel model)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			model.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var payment = await _paymentService.CreatePaymentAsync(model);
			return Ok(payment);
		}

		[HttpPut("payment-status/{paymentId}")]
		[Authorize(Roles = RolesHelper.Admin)]
		public async Task<IActionResult> UpdatePaymentStatus(int paymentId, [FromQuery] string newStatus)
		{
			var result = await _paymentService.UpdatePaymentStatusAsync(paymentId, newStatus);
			return result ? Ok("Payment updated") : NotFound("Payment not found");
		}
	}
}
