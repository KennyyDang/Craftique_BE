using CraftiqueBE.Data.Models.AttributeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Interfaces
{
	public interface IAttributeServices
	{
		Task<Data.Entities.Attribute> AddAsync(Data.Entities.Attribute attribute);
		Task<Data.Entities.Attribute> DeleteAsync(int id);
		Task<IEnumerable<Data.Entities.Attribute>> GetAllAsync();
		Task<Data.Entities.Attribute> GetByIdAsync(int id);
		Task<Data.Entities.Attribute> UpdateAsync(int id, UpdateAttributeModel newAttribute);
	}
}
