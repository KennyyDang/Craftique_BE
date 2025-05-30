using CraftiqueBE.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Extensions
{
	public static class OrderExtensions
	{
		public static IQueryable<Order> FilterByStatus(this IQueryable<Order> query, string? status)
		{
			return !string.IsNullOrWhiteSpace(status) ? query.Where(o => o.OrderStatus == status) : query;
		}

		public static IQueryable<Order> FilterByYear(this IQueryable<Order> query, int? year)
		{
			return year.HasValue ? query.Where(o => o.OrderDate.Year == year.Value) : query;
		}

		public static IQueryable<Order> FilterByQuarter(this IQueryable<Order> query, int? quarter, int? year)
		{
			if (quarter.HasValue)
			{
				int selectedYear = year ?? DateTime.UtcNow.Year;
				int startMonth = (quarter.Value - 1) * 3 + 1;
				int endMonth = startMonth + 2;

				query = query.Where(o =>
					o.OrderDate.Year == selectedYear &&
					o.OrderDate.Month >= startMonth &&
					o.OrderDate.Month <= endMonth);
			}
			return query;
		}


		public static IQueryable<Order> FilterByMonth(this IQueryable<Order> query, int? month, int? year)
		{
			if (month.HasValue)
			{
				int selectedYear = year ?? DateTime.UtcNow.Year;
				query = query.Where(o => o.OrderDate.Year == selectedYear && o.OrderDate.Month == month.Value);
			}
			return query;
		}

		public static IQueryable<Order> FilterByDay(this IQueryable<Order> query, int? day, int? month, int? year)
		{
			if (day.HasValue)
			{
				int selectedYear = year ?? DateTime.UtcNow.Year;
				int selectedMonth = month ?? DateTime.UtcNow.Month;
				query = query.Where(o => o.OrderDate.Year == selectedYear && o.OrderDate.Month == selectedMonth && o.OrderDate.Day == day.Value);
			}
			return query;
		}
	}
}
