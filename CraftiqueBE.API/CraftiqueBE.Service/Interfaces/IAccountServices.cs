using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Models.AuthenticationModel;
using CraftiqueBE.Data.ViewModels.AuthenticationVM;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Interfaces
{
	public interface IAccountServices
	{
		Task<string> CreateAccessToken(User user);
		Task<string> CreateRefreshToken(User user);
		//Task<JwtViewModel> GoogleLoginAsync(GoogleAuthModel model);
		Task<JwtViewModel> LoginAsync(LoginModel model);
		Task<IdentityResult> RegisterAsync(User user);
		Task<JwtViewModel> ValidateRefreshToken(RefreshTokenModel model);
		Task ValidateToken(TokenValidatedContext context);
	}
}
