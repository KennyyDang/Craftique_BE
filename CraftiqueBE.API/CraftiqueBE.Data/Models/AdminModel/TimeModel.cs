using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.AdminModel
{
	public class TimeModel : IValidatableObject
	{
		public int? Year { get; set; }

		[Range(1, 4, ErrorMessage = "Quarter must be between 1 and 4.")]
		public int? Quarter { get; set; }

		[Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
		public int? Month { get; set; }

		public int? Day { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var currentDate = DateTime.UtcNow; // Lấy ngày hiện tại

			int selectedYear = Year ?? currentDate.Year;
			int selectedMonth = Month ?? currentDate.Month;

			if (Day.HasValue)
			{
				int maxDays = DateTime.DaysInMonth(selectedYear, selectedMonth);
				if (Day.Value < 1 || Day.Value > maxDays)
				{
					yield return new ValidationResult(
						$"Day must be between 1 and {maxDays} for {selectedMonth}/{selectedYear}.",
						new[] { nameof(Day) });
				}
			}
		}
	}
}
