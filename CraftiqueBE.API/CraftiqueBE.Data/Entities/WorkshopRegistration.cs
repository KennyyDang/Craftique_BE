using CraftiqueBE.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Entities
{
	public class WorkshopRegistration : IBaseEntity
	{
		[Key]
		public int Id { get; set; }

		[Required, MaxLength(255)]
		public string FullName { get; set; }

		[Required, MaxLength(255)]
		public string Email { get; set; }

		[MaxLength(20)]
		public string? PhoneNumber { get; set; }

		[Required, MaxLength(255)]
		public string WorkshopName { get; set; }

		public DateTime RegisteredDate { get; set; }

		[MaxLength(100)]
		public string Status { get; set; } = "CHỜ XÁC NHẬN";

		public bool IsDeleted { get; set; }
	}
}
