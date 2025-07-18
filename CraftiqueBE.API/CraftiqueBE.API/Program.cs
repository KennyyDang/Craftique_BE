using System.Text;
using AutoMapper;
using CraftiqueBE.API.CustomTokenProviders;
using CraftiqueBE.API.MiddleWares;
using CraftiqueBE.API.Hubs;
using CraftiqueBE.Data;
using CraftiqueBE.Data.Data;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Mapping;
using CraftiqueBE.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using CraftiqueBE.Data.Models.PayOsModel;


namespace CraftiqueBE.API
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			//builder.WebHost.ConfigureKestrel(serverOptions =>
			//{
			//	serverOptions.ListenAnyIP(8080);  // Fly.io yêu cầu listen cổng 8080
			//});


			// -------------------- CONFIGURE SERVICES --------------------

			// Database Context
			builder.Services.AddDbContext<CraftiqueDBContext>(options =>
				options.UseSqlServer(
					builder.Configuration.GetConnectionString("DefaultConnection"),
					b => b.MigrationsAssembly(typeof(CraftiqueDBContext).Assembly.FullName)));

            // Identity
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

			// JWT Authentication
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
				})
				.AddGoogle(options =>
				{
					options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
					options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
				});

			builder.Services.AddAuthorization();

			// Swagger
			builder.Services.AddSwaggerGen(option =>
			{
				option.SwaggerDoc("v1", new OpenApiInfo { Title = "Craftique API", Version = "v1" });
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
						Array.Empty<string>()
					}
				});
			});

			// CORS
			//builder.Services.AddCors(options =>
			//{
			//	options.AddDefaultPolicy(policy =>
			//	{
			//		policy.WithOrigins("http://localhost:3000")
			//			  .AllowAnyHeader()
			//			  .AllowAnyMethod()
			//			  .AllowCredentials();
			//	});
			//});

			builder.Services.AddCors(options =>
			{
				options.AddDefaultPolicy(policy =>
				{
					policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "https://craftique-shop.vercel.app", "https://craftique-admin.vercel.app")
						  .AllowAnyHeader()
						  .AllowAnyMethod()
						  .AllowCredentials();
				});
			});

			// SignalR
			builder.Services.AddSignalR(options =>
			{
				options.EnableDetailedErrors = true;
			});

			// Application Services
			builder.Services
				.AddRepository(builder.Configuration)
				.AddServices();

			builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();

			// PayOS Configuration
			builder.Services.Configure<PayOSConfig>(builder.Configuration.GetSection("PayOS"));

			// -------------------- BUILD APP --------------------

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

			// Luôn bật Swagger (test Fly.io)
			//app.UseSwagger();
			//app.UseSwaggerUI();


			app.UseMiddleware<ExceptionHandlingMiddleware>();

			app.UseCors();
			app.UseStaticFiles();
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapHub<ChatHub>("/chatHub");
			app.MapHub<NotificationHub>("/notificationHub");
			app.MapControllers();

            //Initialize Database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<CraftiqueDBContext>();
                var logger = services.GetRequiredService<ILogger<Program>>();

                // Retry logic
                var retryCount = 0;
                var maxRetries = 30;

                while (retryCount < maxRetries)
                {
                    try
                    {
                        logger.LogInformation("Attempting to connect to database... (Attempt {RetryCount}/{MaxRetries})", retryCount + 1, maxRetries);
                        context.Database.Migrate();
                        logger.LogInformation("Database migration completed successfully.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        retryCount++;
                        logger.LogWarning(ex, "Database connection failed. Retrying in 2 seconds... ({RetryCount}/{MaxRetries})", retryCount, maxRetries);

                        if (retryCount >= maxRetries)
                        {
                            logger.LogError("Failed to connect to database after {MaxRetries} attempts.", maxRetries);
                            throw;
                        }

                        await Task.Delay(TimeSpan.FromSeconds(2));
                    }
                }

                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                await DbInitializer.Initialize(context, userManager, roleManager);
            }

            app.Run();
		}
	}
}
