using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Helper.EmailHelper;
using CraftiqueBE.Data.Models.AuthenticationModel;
using CraftiqueBE.Data.Models.EmailModel;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace CraftiqueBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly IAccountServices _accountServices;
		private readonly UserManager<User> _userManager;
		private readonly IEmailHelper _emailHelper;
		private readonly IMapper _mapper;
		private readonly IConfiguration _configuration;

		public AccountController(IAccountServices accountServices, UserManager<User> userManager, IEmailHelper emailHelper, IMapper mapper, IConfiguration configuration)
		{
			_accountServices = accountServices;
			_userManager = userManager;
			_emailHelper = emailHelper;
			_mapper = mapper;
			_configuration = configuration;
		}

		[HttpPost("Login")]
		public async Task<IActionResult> Login(LoginModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var result = await _accountServices.LoginAsync(model);

			return Ok(result);
		}

		[HttpPost("GoogleLogin")]
		public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthModel model)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var result = await _accountServices.GoogleLoginAsync(model);

				if (result == null)
				{
					return Unauthorized("Invalid Google token");
				}

				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
			}
		}

		[HttpPost("Register")]
		public async Task<IActionResult> Register(CancellationToken cancellationToken, RegisterModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var user = _mapper.Map<User>(model);

			var result = await _accountServices.RegisterAsync(user);
			if (result.Succeeded)
			{
				string confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
				string url = Url.Action("ConfirmEmail", "Account", new { userEmail = user.Email, token = confirmToken }, Request.Scheme);


				string emailBody = $@"
            <html>
            <body style='font-family: Arial, sans-serif; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #007bff;'>Welcome to Craftique Shop!</h2>
                    <p>Hello {model.Email},</p>
                    <p>Thank you for registering with Craftique Shop. To complete your registration, please confirm your email by clicking the button below:</p>
                    <a href='{url}' style='display: inline-block; padding: 10px 20px; background-color: #28a745; color: white; text-decoration: none; border-radius: 5px; margin: 10px 0;'>Confirm Your Email</a>
                    //<p>If the button doesn't work, you can also copy and paste this link into your browser:</p>
                    //<p style='word-break: break-all;'><small>{url}</small></p>
                    <p>We’re excited to have you as part of the Craftique Shop community!</p>
                    <p>Best regards,<br>The Craftique Team</p>
                    <hr style='border: 1px solid #eee;'>
                    <p style='font-size: 12px; color: #777;'>This is an automated email, please do not reply directly.</p>
                </div>
            </body>
            </html>";

				await _emailHelper.SendMailAsync(cancellationToken, new EmailRequestModel
				{
					To = model.Email,
					Subject = "Confirm Email for Register",
					Body = emailBody
				});
				return Ok(result);
			}

			return BadRequest(result.Errors);
		}

		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken(RefreshTokenModel refreshTokenModel)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				if (refreshTokenModel == null) return BadRequest("Could not get refresh token");

				var result = await _accountServices.ValidateRefreshToken(refreshTokenModel);
				if (result == null)
				{
					return Unauthorized("Invalid email or password");
				}

				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
			}
		}

		[HttpGet("forget-password")]
		public async Task<IActionResult> ForgetPassword(CancellationToken cancellationToken, string email)
		{
			var user = await _userManager.FindByEmailAsync(email);

			if (user == null)
			{
				return BadRequest("Email not exist");
			}

			string resetPasswordUrl = _configuration.GetValue<string>("ResetPasswordUrl");

			string token = await _userManager.GeneratePasswordResetTokenAsync(user);

			string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

			string resetPasswordUrlFull = $"{resetPasswordUrl}?email={email}&token={encodedToken}";

			string emailBody = $"Please reset your password by clicking here: <a href='{resetPasswordUrlFull}'>click here</a><p style='word-break: break-all;'><small>{token}</small></p>?";

			await _emailHelper.SendMailAsync(cancellationToken, new EmailRequestModel
			{
				To = email,
				Subject = "Confirm Email for reset password",
				Body = emailBody
			});

			return Ok("Please check your email");
		}

		[HttpGet("confirm-email")]
		public async Task<IActionResult> ConfirmEmail(string userEmail, string token)
		{
			var user = await _userManager.FindByEmailAsync(userEmail);

			if (user == null)
			{
				return BadRequest("Account is not exist in system!!");
			}

			if (user.EmailConfirmed)
			{
				return Ok("The email has already");
			}

			var result = await _userManager.ConfirmEmailAsync(user, token);

			if (result.Succeeded)
			{
				return Ok("Your account has been actived");
			}

			return BadRequest($"confirm email failed: {result.Errors}");
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var user = await _userManager.FindByEmailAsync(model.Email);

			if (user == null)
			{
				return BadRequest("Email not exist");
			}

			var result = await _userManager.ResetPasswordAsync(user, model.ResetPasswordToken, model.ConfirmPassword);

			if (result.Succeeded)
			{
				return Ok("reset password successful");
			}
			return BadRequest("reset password fail");
		}
	}
}
