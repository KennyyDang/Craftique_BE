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
			return service;

		}
	}
}
