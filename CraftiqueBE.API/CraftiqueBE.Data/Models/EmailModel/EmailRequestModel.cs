using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.EmailModel
{
	public class EmailRequestModel
	{
		public string To { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public string[] AttachmentFilePaths { get; set; } = Array.Empty<string>();
	}
}
