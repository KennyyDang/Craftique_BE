using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.WorkshopRegistrationVM
{
	public class WorkshopRegistrationViewModel
	{
		public int Id { get; set; }
		public string FullName { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string WorkshopName { get; set; }
		public DateTime RegisteredDate { get; set; }
		public string Status { get; set; }
	}
}
