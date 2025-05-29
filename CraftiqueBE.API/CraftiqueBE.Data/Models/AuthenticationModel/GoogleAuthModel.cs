using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.AuthenticationModel
{
	public class GoogleAuthModel
	{
		public string IdToken { get; set; }
		public string Email { get; set; }
		public string Name { get; set; }
		public string PhoneNumber { get; set; }
	}
}
