using CraftiqueBE.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Entities
{
	public class UserDesignUpload : IBaseEntity
	{
		[Key]
		public int UserDesignUploadID { get; set; }

		[Required]
		public string UserId { get; set; } // nếu có liên kết đến bảng User

		[Required]
		[MaxLength(1000)]
		public string FileUrl { get; set; }

		[MaxLength(255)]
		public string FileName { get; set; }

		public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

		public bool IsDeleted { get; set; }
	}
}
