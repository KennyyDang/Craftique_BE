using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.ChatVM
{
	public class ChatMessageViewModel
	{
		public int ChatID { get; set; }
		public int ChatRoomID { get; set; }
		public string SenderID { get; set; }
		public string SenderName { get; set; }
		public string Content { get; set; }
		public bool IsRead { get; set; }
		public DateTime CreatedDate { get; set; }
		public bool IsDeleted { get; set; }
	}
}
