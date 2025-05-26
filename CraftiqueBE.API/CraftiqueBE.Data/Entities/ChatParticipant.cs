using CraftiqueBE.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Entities
{
	public class ChatParticipant : IBaseEntity
	{
		[Key]
		public int ID { get; set; }

		[ForeignKey("ChatRoom")]
		public int ChatRoomID { get; set; }

		[ForeignKey("User")]
		public string UserID { get; set; }

		public bool IsAdmin { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime CreatedDate { get; set; }

		public virtual User User { get; set; }
		public virtual ChatRoom ChatRoom { get; set; }
	}
}
