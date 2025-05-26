using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Interfaces
{
	public interface IBaseEntity
	{
		bool IsDeleted { get; set; }
	}
}
