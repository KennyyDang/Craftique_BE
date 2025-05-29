using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CraftiqueBE.Data.Data;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data;
using CraftiqueBE.Data.Mapping;


namespace CraftiqueBE.API
{
	public class Program
	{
		public static async Task Main(string[] args) // Changed return type to Task and added async modifier
		{
			// Tạo builder cho ứng dụng
			var builder = WebApplication.CreateBuilder(args);

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
			// Đăng ký Identity (User & Role)
			builder.Services.AddIdentity<CraftiqueBE.Data.Entities.User, IdentityRole>()
				.AddEntityFrameworkStores<CraftiqueBE.Data.CraftiqueDBContext>()
				.AddDefaultTokenProviders();
			builder.Services
				.AddRepository(builder.Configuration);
				//.AddServices();
			builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
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