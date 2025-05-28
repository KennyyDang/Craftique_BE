using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.UserModel
{
	public class ChangeRoleModel
	{
		[Required(ErrorMessage = "New role is required")]
		public string NewRole { get; set; }
	}
}
