using CraftiqueBE.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Entities
{
	public class Role : IBaseEntity
	{
		[Key]
		public int RoleID { get; set; }

		[Required]
		[MaxLength(255)]
		public string RoleName { get; set; }
		public bool IsDeleted { get; set; }

		public virtual ICollection<User> Users { get; set; }
	}
}
