using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.Models.BlogModel;
using CraftiqueBE.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Services
{
	public class BlogServices : IBlogServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public BlogServices(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<IEnumerable<Blog>> GetAllAsync(string searchTitle = null)
		{
			var result = await _unitOfWork.BlogRepository.GetAllAsync(
				predicate: null,
				includes: b => b.BlogImages
			);

			return result?
				.Where(b => !b.IsDeleted && (string.IsNullOrEmpty(searchTitle) || b.Title.Contains(searchTitle, StringComparison.OrdinalIgnoreCase)))
				.Select(b =>
				{
					b.BlogImages = b.BlogImages.Where(bi => !bi.IsDeleted).ToList();
					return b;
				})
				.ToList() ?? new List<Blog>();
		}

		public async Task<Blog> GetByIdAsync(int id)
		{
			var blog = await _unitOfWork.BlogRepository.GetByIdAsync(
				id,
				includes: b => b.BlogImages
			);

			if (blog == null || blog.IsDeleted)
				throw new KeyNotFoundException("Blog not found");

			blog.BlogImages = blog.BlogImages.Where(bi => !bi.IsDeleted).ToList();

			return blog;
		}

		public async Task<Blog> AddAsync(Blog blog)
		{
			await _unitOfWork.BeginTransactionAsync();
			try
			{
				var product = await _unitOfWork.ProductRepository.GetByIdAsync(blog.ProductId);
				if (product == null)
					throw new KeyNotFoundException($"Product with ID {blog.ProductId} not found.");

				blog.UploadDate = DateTime.Now;
				blog.UpdateDate = DateTime.Now;
				blog.View = 0;
				blog.Like = 0;

				var blogImages = blog.BlogImages?.ToList();
				blog.BlogImages = null;

				var result = await _unitOfWork.BlogRepository.AddAsync(blog);
				await _unitOfWork.SaveChangesAsync();

				bool IsInvalid(string value) => string.IsNullOrWhiteSpace(value) || value == "string";

				if (blogImages != null && blogImages.Any())
				{
					var validImages = blogImages
						.Where(img => !IsInvalid(img.ImageUrl))
						.ToList();

					foreach (var image in validImages)
					{
						var blogImage = new BlogImage
						{
							BlogId = result.BlogID,
							ImageUrl = image.ImageUrl,
							IsDeleted = false
						};
						await _unitOfWork.BlogImageRepository.AddAsync(blogImage);
					}

					await _unitOfWork.SaveChangesAsync();
				}

				await _unitOfWork.CommitTransactionAsync();

				return await GetByIdAsync(result.BlogID);
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw new Exception($"Failed to save blog: {ex.Message}", ex);
			}
		}

		public async Task<Blog> UpdateAsync(int id, UpdateBlogModel newBlog)
		{
			await _unitOfWork.BeginTransactionAsync();
			try
			{
				var blog = await _unitOfWork.BlogRepository.GetByIdAsync(id);
				if (blog == null || blog.IsDeleted)
					throw new KeyNotFoundException($"Blog with ID {id} not found.");

				var product = await _unitOfWork.ProductRepository.GetByIdAsync(newBlog.ProductId);
				if (product == null)
					throw new KeyNotFoundException($"Product with ID {newBlog.ProductId} not found.");

				bool IsInvalid(string value) => string.IsNullOrWhiteSpace(value) || value == "string";

				if (!IsInvalid(newBlog.Title))
				{
					blog.Title = newBlog.Title;
				}
				if (!IsInvalid(newBlog.Content))
				{
					blog.Content = newBlog.Content;
				}
				if (!IsInvalid(newBlog.Author))
				{
					blog.Author = newBlog.Author;
				}

				blog.ProductId = newBlog.ProductId;
				blog.UpdateDate = DateTime.Now;

				if (newBlog.BlogImages != null && newBlog.BlogImages.Any() && newBlog.BlogImages.ToString() != "string")
				{
					var validImages = newBlog.BlogImages
						.Where(img => !IsInvalid(img.ImageUrl))
						.ToList();

					if (validImages.Any())
					{
						await _unitOfWork.BlogImageRepository.HardRemove(bi => bi.BlogId == id);

						foreach (var imageModel in validImages)
						{
							var blogImage = new BlogImage
							{
								BlogId = id,
								ImageUrl = imageModel.ImageUrl,
								IsDeleted = false
							};
							await _unitOfWork.BlogImageRepository.AddAsync(blogImage);
						}
					}
				}

				var result = await _unitOfWork.BlogRepository.Update(blog);
				if (!result)
				{
					throw new InvalidOperationException("Failed to update blog.");
				}

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();
				return blog;
			}
			catch
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}

		public async Task<Blog> DeleteAsync(int id)
		{
			await _unitOfWork.BeginTransactionAsync();
			try
			{
				var blog = await _unitOfWork.BlogRepository.GetByIdAsync(id);
				if (blog == null || blog.IsDeleted)
					throw new KeyNotFoundException($"Blog with ID {id} not found.");

				var result = await _unitOfWork.BlogRepository.SoftDelete(blog);
				if (!result)
				{
					throw new InvalidOperationException("Failed to delete blog.");
				}

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();
				return blog;
			}
			catch
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}
	}
}
