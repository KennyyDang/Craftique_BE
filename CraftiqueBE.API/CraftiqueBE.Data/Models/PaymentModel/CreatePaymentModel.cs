using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Models.WalletModel
{
	public class CreatePaymentModel
	{
		[Required]
		public decimal Amount { get; set; }
		//public string UserId { get; set; }
	}
}
