using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Helper.EmailHelper;
using CraftiqueBE.Data.Helper;
using CraftiqueBE.Data.Models.AuthenticationModel;
using CraftiqueBE.Data.ViewModels.AuthenticationVM;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth;
using CraftiqueBE.Data.Models.EmailModel;

namespace CraftiqueBE.Service.Services
{
	public class AccountServices : IAccountServices
	{
		private readonly IConfiguration _configuration;
		private readonly IUserServices _userServices;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<User> _userManager;
		private readonly IEmailHelper _emailHelper;

		public AccountServices(IConfiguration configuration, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IUserServices userServices, IEmailHelper emailHelper)
		{
			_configuration = configuration;
			_userManager = userManager;
			_roleManager = roleManager;
			_userServices = userServices;
			_emailHelper = emailHelper;
		}

		public async Task<JwtViewModel> LoginAsync(LoginModel model)
		{

			var existingUser = await _userManager.FindByEmailAsync(model.Email);
			if (existingUser == null) throw new UnauthorizedAccessException("Invalid email or password");

			var isCorrect = await _userManager.CheckPasswordAsync(existingUser, model.Password);
			if (!isCorrect) throw new UnauthorizedAccessException("Invalid email or password");

			if (existingUser.IsDeleted == true) throw new UnauthorizedAccessException("You have been banned");

			var accessToken = await CreateAccessToken(existingUser);
			var refreshToken = await CreateRefreshToken(existingUser);

			return new JwtViewModel
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken,
				UserID = existingUser.Id,
				UserName = existingUser.UserName,
				Name = existingUser.Name,
			};
		}

		public async Task<JwtViewModel> GoogleLoginAsync(GoogleAuthModel model)
		{
			// Xác thực token từ Google
			var settings = new GoogleJsonWebSignature.ValidationSettings()
			{
				Audience = new List<string> { _configuration["Authentication:Google:ClientId"] }
			};

			var payload = await GoogleJsonWebSignature.ValidateAsync(model.IdToken, settings);
			if (payload == null)
				return null;

			// Kiểm tra xem user đã tồn tại chưa
			var user = await _userManager.FindByEmailAsync(payload.Email);

			if (user == null)
			{
				// Tạo user mới nếu chưa tồn tại
				user = new User
				{
					UserName = payload.Email,
					Email = payload.Email,
					Name = model.Name,
					PhoneNumber = model.PhoneNumber,
					EmailConfirmed = payload.EmailVerified
				};

				var password = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
				var result = await _userManager.CreateAsync(user, password);

				if (!result.Succeeded)
				{
					throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
				}

				await AssignRoleAsync(user, RolesHelper.Customer);
			}
			else
			{
				bool isUpdated = false;

				if (user.Name != model.Name)
				{
					user.Name = model.Name;
					isUpdated = true;
				}

				if (!string.IsNullOrEmpty(model.PhoneNumber) && user.PhoneNumber != model.PhoneNumber)
				{
					user.PhoneNumber = model.PhoneNumber;
					isUpdated = true;
				}

				if (isUpdated)
				{
					await _userManager.UpdateAsync(user);
				}
			}

			// Tạo JWT token
			var accessToken = await CreateAccessToken(user);
			var refreshToken = await CreateRefreshToken(user);

			return new JwtViewModel
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken,
				UserName = user.UserName,
				Name = user.Name,
			};
		}

		public async Task<IdentityResult> RegisterAsync(User user)
		{

			var result = await _userManager.CreateAsync(user, user.PasswordHash);

			if (!result.Succeeded)
			{
				throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
			}

			await AssignRoleAsync(user, RolesHelper.Customer);

			return result;
		}

		public async Task<string> CreateAccessToken(User user)
		{
			DateTime expiredDate = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JWT:AccessTokenExpiredByMinutes"]));
			var userRoles = await _userManager.GetRolesAsync(user);

			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique ID của token
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64), // Thời gian phát hành
                new Claim(JwtRegisteredClaimNames.Exp, ((DateTimeOffset)expiredDate).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64), // Thời gian hết hạn
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.String),
				new Claim(ClaimTypes.Name, user.Name),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
			};

			foreach (var role in userRoles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
			var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var tokenInfo = new JwtSecurityToken(
					issuer: _configuration["JWT:Issuer"],
					audience: _configuration["JWT:Audience"],
					claims: claims,
					notBefore: DateTime.UtcNow,
					expires: expiredDate,
					signingCredentials: credential
				);

			string token = new JwtSecurityTokenHandler().WriteToken(tokenInfo);

			return token;
		}

		public async Task<string> CreateRefreshToken(User user)
		{
			var code = Guid.NewGuid().ToString();
			var expiredDate = DateTime.UtcNow.AddHours(int.Parse(_configuration["JWT:RefreshTokenExpiredByHours"]));
			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Jti, code), // Unique ID của token
                new Claim(JwtRegisteredClaimNames.Exp, ((DateTimeOffset)expiredDate).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64), // Thời gian hết hạn
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
			var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var tokenInfo = new JwtSecurityToken(
					issuer: _configuration["JWT:Issuer"],
					audience: _configuration["JWT:Audience"],
					claims: claims,
					notBefore: DateTime.UtcNow,
					expires: expiredDate,
					signingCredentials: credential
				);

			string token = new JwtSecurityTokenHandler().WriteToken(tokenInfo);

			await _userManager.SetAuthenticationTokenAsync(user, "REFRESHTOKENPROVIDER", "RefreshToken", code);

			return token;
		}

		public async Task ValidateToken(TokenValidatedContext context)
		{
			var claims = context.Principal.Claims.ToList();

			//check claims in token
			if (claims.Count() == 0)
			{
				context.Fail("This token contains no information");
				return;
			}

			var identity = context.Principal?.Identity as ClaimsIdentity;
			if (identity == null)
			{
				context.Fail("Invalid token.");
				return;
			}

			//check email
			var emailClaim = identity.FindFirst(JwtRegisteredClaimNames.Email) ?? identity.FindFirst(ClaimTypes.Email);
			if (emailClaim != null)
			{
				string email = emailClaim.Value;
				var user = await _userServices.FindByEmail(email);
				if (user == null)
				{
					context.Fail($"Invalid email: {email}");
					return;
				}
			}
			else
			{
				context.Fail("Email claim is missing.");
				return;
			}

			if (identity.FindFirst(JwtRegisteredClaimNames.Exp) != null)
			{
				var dateExp = identity.FindFirst(JwtRegisteredClaimNames.Exp).Value;

				long ticks = long.Parse(dateExp);
				var expirationDate = DateTimeOffset.FromUnixTimeSeconds(ticks).UtcDateTime;

				if (DateTime.UtcNow > expirationDate)
				{
					context.Fail("This token is expired.");
					return;
				}
			}
			else
			{
				context.Fail("Exp claim is missing.");
				return;
			}
		}

		public async Task<JwtViewModel> ValidateRefreshToken(RefreshTokenModel model)
		{
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var key = Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]);

				var claimPriciple = tokenHandler.ValidateToken(model.RefreshToken, new TokenValidationParameters
				{
					ValidateIssuer = false,
					ValidateAudience = false,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuerSigningKey = true,
					ValidateLifetime = true,
					ClockSkew = TimeSpan.Zero
				}, out _);

				if (claimPriciple == null) return null;

				var identity = claimPriciple.Identity as ClaimsIdentity;
				var emailClaim = identity.FindFirst(JwtRegisteredClaimNames.Email) ?? identity.FindFirst(ClaimTypes.Email);
				string email = emailClaim?.Value;
				string jti = claimPriciple.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

				if (email == null || jti == null) return null;

				var user = await _userManager.FindByEmailAsync(email);
				if (user == null) return null;

				var storedJti = await _userManager.GetAuthenticationTokenAsync(user, "REFRESHTOKENPROVIDER", "RefreshToken");

				if (string.IsNullOrEmpty(storedJti))
				{
					return null; // Token đã bị xóa hoặc ng dùng đăng xuất
				}

				if (storedJti != jti)
				{
					return null;
				}

				var newAccessToken = await CreateAccessToken(user);
				var newRefreshToken = await CreateRefreshToken(user);

				return new JwtViewModel
				{
					AccessToken = newAccessToken,
					RefreshToken = newRefreshToken,
					UserName = user.UserName,
					Name = user.Name
				};
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		private async Task AssignRoleAsync(User user, string roleName)
		{
			if (!await _roleManager.RoleExistsAsync(roleName))
			{
				await _roleManager.CreateAsync(new IdentityRole(roleName));
			}
			await _userManager.AddToRoleAsync(user, roleName);
		}
	}
}
