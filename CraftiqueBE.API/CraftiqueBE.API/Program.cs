using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CraftiqueBE.Data.Data;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data;
using CraftiqueBE.Data.Mapping;
using CraftiqueBE.Service;
using CraftiqueBE.API.MiddleWares;
using CraftiqueBE.API.CustomTokenProviders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Google;
using System.Text;
using Microsoft.OpenApi.Models;


namespace CraftiqueBE.API
{
	public class Program
	{
		public static async Task Main(string[] args) // Changed return type to Task and added async modifier
		{
			// Tạo builder cho ứng dụng
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddSignalR(options =>
			{
				options.EnableDetailedErrors = true;
			});

			// Swagger Configuration
			builder.Services.AddSwaggerGen(option =>
			{
				option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
				option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					In = ParameterLocation.Header,
					Description = "Please enter a valid token",
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					BearerFormat = "JWT",
					Scheme = "Bearer"
				});
				option.AddSecurityRequirement(new OpenApiSecurityRequirement
				 {
					 {
						 new OpenApiSecurityScheme
						 {
							 Reference = new OpenApiReference
							 {
								 Type = ReferenceType.SecurityScheme,
								 Id = "Bearer"
							 }
						 },
						 new string[]{}
					 }
				 });
			});

			// Đăng ký DbContext và chỉ định Migrations Assembly
			builder.Services.AddDbContext<CraftiqueBE.Data.CraftiqueDBContext>(options =>
				options.UseSqlServer(
					builder.Configuration.GetConnectionString("DefaultConnection"),
					b => b.MigrationsAssembly("CraftiqueBE.Data")
				));

			// Các dịch vụ khác
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddCors(options =>
			{
				options.AddDefaultPolicy(policy =>
				{
					policy.WithOrigins("http://localhost:3000")
						  .AllowAnyHeader()
						  .AllowAnyMethod()
						  .AllowCredentials();
				});
			});

			// Đăng ký Identity (User & Role)
			builder.Services.AddIdentity<User, IdentityRole>(options =>
			{
				options.Password.RequiredLength = 1;
				options.Password.RequireUppercase = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireDigit = false;
				options.Password.RequireNonAlphanumeric = false;

				options.User.RequireUniqueEmail = true;

				options.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";
			})
				.AddEntityFrameworkStores<CraftiqueDBContext>()
				.AddDefaultTokenProviders()
				.AddTokenProvider<DataProtectorTokenProvider<User>>("REFRESHTOKENPROVIDER")
				.AddTokenProvider<EmailConfirmationTokenProvider<User>>("emailconfirmation");


			builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
				opt.TokenLifespan = TimeSpan.FromHours(2));
			builder.Services.Configure<EmailConfirmationTokenProviderOptions>(opt =>
				opt.TokenLifespan = TimeSpan.FromDays(3));
			//add jwt
			builder.Services
				.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(x =>
				{
					x.SaveToken = true;
					x.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = false,
						ValidateAudience = false,
						ValidateLifetime = true,
						ValidIssuer = builder.Configuration["JWT:Issuer"],
						ValidAudience = builder.Configuration["JWT:Audience"],
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
					};

					// Add SignalR specific JWT configuration
					x.Events = new JwtBearerEvents
					{
						OnMessageReceived = context =>
						{
							var accessToken = context.Request.Query["access_token"];
							var path = context.HttpContext.Request.Path;

							if (!string.IsNullOrEmpty(accessToken) &&
								(path.StartsWithSegments("/chatHub") || path.StartsWithSegments("/notificationHub")))
							{
								context.Token = accessToken;
							}
							return Task.CompletedTask;
						}
					};
				});
				// Thêm Google Authentication nếu muốn sử dụng OAuth chuẩn của ASP.NET
				//.AddGoogle(options =>
				//{
				//	options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
				//	options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
				//});

				builder.Services.AddAuthorization();



			builder.Services
				.AddRepository(builder.Configuration)
				.AddServices();
			builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}
			else
			{
				app.UseHttpsRedirection();
			}

			app.UseMiddleware<ExceptionHandlingMiddleware>();
			app.UseCors();
			app.UseAuthentication(); // Đặt UseAuthentication trước UseAuthorization
			app.UseAuthorization();
			app.MapControllers();

			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var context = services.GetRequiredService<CraftiqueDBContext>();
				var userManager = services.GetRequiredService<UserManager<User>>();
				var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

				await DbInitializer.Initialize(context, userManager, roleManager); 
			}

			app.Run();
		}
	}
}