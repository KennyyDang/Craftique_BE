﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Helper.EmailHelper
{
	public class EmailConfiguration
	{
		public string? DefaultSender { get; set; }
		public string? Password { get; set; }
		public string? DisplayName { get; set; }
		public string? Provider { get; set; }
		public int Port { get; set; }

	}
}
