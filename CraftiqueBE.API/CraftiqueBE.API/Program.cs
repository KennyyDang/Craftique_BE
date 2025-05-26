using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace CraftiqueBE.API
{
	public class Program
	{
		public static void Main(string[] args)
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


			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseAuthorization();
			app.MapControllers();

			app.Run();
		}
	}
}