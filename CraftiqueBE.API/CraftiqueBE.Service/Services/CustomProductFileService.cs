using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.Models.CustomProductModel;
using CraftiqueBE.Data.ViewModels.CustomProductVM;
using CraftiqueBE.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Services
{
	public class CustomProductFileService : ICustomProductFileService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly string _uploadFolder = "uploads/custom-products"; // Bạn sửa theo cấu hình của bạn
		private readonly string[] _allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

		public CustomProductFileService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<CustomProductFileViewModel> UploadFileAsync(CustomProductFileUploadModel model, string userId)
		{
			if (model.File == null && string.IsNullOrEmpty(model.CustomText))
				throw new ArgumentException("Phải upload file hoặc nhập text.");

			string? fileUrl = null;
			string? fileName = null;

			if (model.File != null)
			{
				var extension = Path.GetExtension(model.File.FileName).ToLowerInvariant();

				if (!_allowedExtensions.Contains(extension))
					throw new ArgumentException("Chỉ chấp nhận các file ảnh có định dạng: jpg, jpeg, png, gif, bmp.");

				fileName = Guid.NewGuid() + extension;
				var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", _uploadFolder, fileName);
				Directory.CreateDirectory(Path.GetDirectoryName(path));
				using (var stream = new FileStream(path, FileMode.Create))
				{
					await model.File.CopyToAsync(stream);
				}
				fileUrl = Path.Combine("/", _uploadFolder, fileName).Replace("\\", "/");
			}

			var entity = new CustomProductFile
			{
				CustomProductID = model.CustomProductID,
				FileUrl = fileUrl,
				FileName = fileName,
				CustomText = model.CustomText,
				UserId = userId,
				UploadedAt = DateTime.UtcNow,
				Quantity = model.Quantity,
				IsDeleted = false
			};

			await _unitOfWork.CustomProductFileRepository.AddAsync(entity);
			await _unitOfWork.SaveChangesAsync();

			return _mapper.Map<CustomProductFileViewModel>(entity);
		}

		public async Task<List<CustomProductFileViewModel>> GetFilesByCustomProductAsync(int customProductId)
		{
			var files = await _unitOfWork.CustomProductFileRepository.GetAllAsync(f => f.CustomProductID == customProductId);
			return _mapper.Map<List<CustomProductFileViewModel>>(files);
		}

		public async Task<List<CustomProductFileViewModel>> GetCustomProductFilesByUserAsync(string userId)
		{
			var files = await _unitOfWork.CustomProductFileRepository.GetAllAsync(f => f.UserId == userId && !f.IsDeleted);
			return _mapper.Map<List<CustomProductFileViewModel>>(files);
		}
		public async Task<bool> DeleteFileAsync(int customProductFileId)
		{
			var file = await _unitOfWork.CustomProductFileRepository.GetByIdAsync(customProductFileId);
			if (file == null) return false;

			file.IsDeleted = true;
			await _unitOfWork.CustomProductFileRepository.Update(file);
			await _unitOfWork.SaveChangesAsync();

			return true;
		}
	}
}
