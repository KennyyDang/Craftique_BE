using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.UserModel
{
	public class UserModel
	{
		public string Name { get; set; }
		public string? Address { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Avatar { get; set; }
	}
}
