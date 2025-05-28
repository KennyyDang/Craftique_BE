using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.AuthenticationVM
{
	public class JwtViewModel
	{
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
		public string UserID { get; set; }
		public string UserName { get; set; }
		public string Name { get; set; }
	}
}
