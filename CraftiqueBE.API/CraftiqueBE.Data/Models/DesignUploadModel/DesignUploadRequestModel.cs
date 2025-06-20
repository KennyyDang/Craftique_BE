using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.DesignUploadModel
{
	public class DesignUploadRequestModel
	{
		public IFormFile File { get; set; } // Dùng cho controller [FromForm]
	}
}
