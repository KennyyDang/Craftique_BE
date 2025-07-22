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

		public CustomProductFileService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<CustomProductFileViewModel> UploadFileAsync(CustomProductFileUploadModel model, string userId)
		{
			if (string.IsNullOrEmpty(model.ImageUrl) && string.IsNullOrEmpty(model.CustomText))
				throw new ArgumentException("Phải nhập link ảnh hoặc nhập text.");

			var entity = new CustomProductFile
			{
				CustomProductID = model.CustomProductID,
				FileUrl = model.ImageUrl, // <-- dùng trực tiếp link
				FileName = null,         // không cần lưu tên file vật lý nữa
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
		public async Task<List<CustomProductFileViewModel>> GetAllFilesAsync()
		{
			var files = await _unitOfWork.CustomProductFileRepository
				.GetAllAsync(
					f => !f.IsDeleted,
					f => f.CustomProduct // include để lấy thông tin custom product
				);

			var result = files.Select(file =>
			{
				var vm = _mapper.Map<CustomProductFileViewModel>(file);
				vm.CustomProductImageUrl = file.CustomProduct?.ImageUrl;
				vm.CustomProductName = file.CustomProduct?.CustomName;
				return vm;
			}).ToList();

			return result;
		}
		public async Task<CustomProductFileViewModel?> GetFileByIdAsync(int id)
		{
			var file = (await _unitOfWork.CustomProductFileRepository
				.GetAllAsync(
					f => f.CustomProductFileID == id && !f.IsDeleted,
					f => f.CustomProduct // Chỉ cần đến đây là đủ
				)).FirstOrDefault();

			if (file == null) return null;

			var vm = _mapper.Map<CustomProductFileViewModel>(file);

			// ✅ Lấy từ CustomProduct, không phải Product
			vm.CustomProductImageUrl = file.CustomProduct?.ImageUrl;
			vm.CustomProductName = file.CustomProduct?.CustomName;

			return vm;
		}
	}
}
