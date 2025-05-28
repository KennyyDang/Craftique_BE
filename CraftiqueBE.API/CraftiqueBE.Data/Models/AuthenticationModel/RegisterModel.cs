using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.AuthenticationModel
{
	public class RegisterModel
	{
		[Required]
		public string Name { get; set; }
		[Required, EmailAddress]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }
		[Required]
		[Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}
}
