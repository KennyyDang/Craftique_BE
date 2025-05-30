using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Models.BlogModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Interfaces
{
	public interface IBlogServices
	{
		Task<Blog> AddAsync(Blog blog);
		Task<Blog> DeleteAsync(int id);
		Task<IEnumerable<Blog>> GetAllAsync(string searchTitle = null);
		Task<Blog> GetByIdAsync(int id);
		Task<Blog> UpdateAsync(int id, UpdateBlogModel newBlog);
	}
}
