using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.ChatVM
{
	public class ChatRoomViewModel
	{
		public int ChatRoomID { get; set; }
		public string RoomName { get; set; }
		public bool IsGroup { get; set; }
		public DateTime CreatedDate { get; set; }
		public List<ChatMessageViewModel> Messages { get; set; }
		public List<ChatParticipantViewModel> Participants { get; set; }
		public ChatMessageViewModel LastMessage { get; set; }
	}
}
