using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.Models.DesignUploadModel;
using CraftiqueBE.Data.ViewModels.UserDesignUploadVM;
using CraftiqueBE.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Services
{
	public class DesignService : IDesignService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IWebHostEnvironment _env;

		public DesignService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_env = env;
		}

		public async Task<DesignUploadViewModel> UploadDesignAsync(string userId, DesignUploadRequestModel request)
		{
			// Save file to /uploads/designs
			var file = request.File;
			var rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
			var uploadsFolder = Path.Combine(rootPath, "uploads", "designs");

			if (!Directory.Exists(uploadsFolder))
				Directory.CreateDirectory(uploadsFolder);
			// Kiểm tra file ảnh 
			var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
			var fileExtension = Path.GetExtension(file.FileName).ToLower();
			if (!allowedExtensions.Contains(fileExtension))
				throw new InvalidOperationException("Chỉ được phép upload file ảnh (.jpg, .png, .jpeg, .gif, .bmp)");

			var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
			var filePath = Path.Combine(uploadsFolder, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			var entity = new UserDesignUpload
			{
				UserId = userId,
				FileName = file.FileName,
				FileUrl = $"/uploads/designs/{fileName}",
				UploadedAt = DateTime.UtcNow,
				IsDeleted = false
			};

			await _unitOfWork.UserDesignUploadRepository.AddAsync(entity);
			await _unitOfWork.SaveChangesAsync();

			return _mapper.Map<DesignUploadViewModel>(entity);
		}

		public async Task<CustomizationViewModel> CreateCustomizationAsync(CreateCustomizationModel model)
		{
			var customization = _mapper.Map<ProductCustomization>(model);
			await _unitOfWork.ProductCustomizationRepository.AddAsync(customization);
			await _unitOfWork.SaveChangesAsync();

			// Load UserDesignUpload để ánh xạ FileUrl nếu cần
			customization.UserDesignUpload = await _unitOfWork.UserDesignUploadRepository
				.GetByIdAsync(model.UserDesignUploadID ?? 0);

			return _mapper.Map<CustomizationViewModel>(customization);
		}
	}
}
