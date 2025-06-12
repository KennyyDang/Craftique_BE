using CraftiqueBE.Service.Interfaces;
using CraftiqueBE.Service.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddServices(this IServiceCollection service)
		{
			// Add HttpClient
			service.AddHttpClient();

			// Existing service registrations
			service.AddScoped<IAccountServices, AccountServices>();
			service.AddScoped<IUserServices, UserServices>();
			service.AddScoped<IAdminServices, AdminServices>();
			service.AddScoped<IOrderServices, OrderServices>();
			service.AddScoped<IAttributeServices, AttributeServices>();
			service.AddScoped<IProductServices, ProductServices>();
			service.AddScoped<IProductItemServices, ProductItemServices>();
			service.AddScoped<IProductItemAttributeServices, ProductItemAttributeServices>();
			service.AddScoped<IProductImgServices, ProductImgServices>();
			service.AddScoped<IBlogServices, BlogServices>();
			service.AddScoped<ICategoryServices, CategoryServices>();
			service.AddScoped<IReviewServices, ReviewServices>();
			service.AddScoped<INotificationServices, NotificationServices>();
			service.AddScoped<IChatServices, ChatServices>();
			service.AddScoped<ICategoryServices, CategoryServices>();
			service.AddScoped<IWalletServices, WalletServices>();
			service.AddScoped<IPaymentServices, PaymentServices>(); 

			return service;
		}
	}
}
