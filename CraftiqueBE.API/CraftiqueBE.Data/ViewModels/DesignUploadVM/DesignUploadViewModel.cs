using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.ViewModels.UserDesignUploadVM
{
	public class DesignUploadViewModel
	{
		public int ID { get; set; }
		public string FileUrl { get; set; }
		public string FileName { get; set; }
		public DateTime UploadedAt { get; set; }
	}
}
