using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.UserVM
{
	public class UserViewModel
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string? Address { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Avatar { get; set; }
		public string Role { get; set; }
	}
}
