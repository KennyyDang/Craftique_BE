using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.ChatVM
{
	public class CreateGroupRoomRequest
	{
		[Required]
		[StringLength(100, MinimumLength = 1)]
		public string Name { get; set; }

		[Required]
		[MinLength(1)]
		public List<string> Participants { get; set; }
	}
}
