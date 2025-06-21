using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.EmailModel
{
	public class SendWorkshopBulkEmailModel
	{
		public List<int> RegistrationIds { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
	}
}
