using CraftiqueBE.Data.Models.EmailModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Helper.EmailHelper
{
	public interface IEmailHelper
	{
		Task SendMailAsync(CancellationToken cancellationToken, EmailRequestModel emailRequest);
	}
}
