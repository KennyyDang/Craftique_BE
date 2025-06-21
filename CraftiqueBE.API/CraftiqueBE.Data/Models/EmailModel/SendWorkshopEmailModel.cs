using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.EmailModel
{
	public class SendWorkshopEmailModel
	{
		public int RegistrationId { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
	}
}
