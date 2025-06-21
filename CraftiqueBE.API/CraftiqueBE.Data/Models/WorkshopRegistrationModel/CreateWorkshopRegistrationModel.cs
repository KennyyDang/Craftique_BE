using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.WorkshopRegistrationModel
{
	public class CreateWorkshopRegistrationModel
	{
		public string FullName { get; set; }
		public string Email { get; set; }
		public string? PhoneNumber { get; set; }
		public string WorkshopName { get; set; }
	}
}
