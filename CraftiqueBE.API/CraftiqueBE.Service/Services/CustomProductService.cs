using AutoMapper;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.Models.CustomProductModel;
using CraftiqueBE.Data.ViewModels.CustomProductVM;
using CraftiqueBE.Service.Interfaces;


namespace CraftiqueBE.Service.Services
{
	public class CustomProductService : ICustomProductService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;


		public CustomProductService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		// Admin thêm custom product
		public async Task<CustomProductViewModel> AddCustomProductAsync(CustomProductCreateModel model)
		{
			var customProduct = new CustomProduct
			{
				ProductID = model.ProductID,
				CustomName = model.CustomName,
				Description = model.Description,
				Price = model.Price,
				IsDeleted = false
			};

			await _unitOfWork.CustomProductRepository.AddAsync(customProduct);
			await _unitOfWork.SaveChangesAsync();

			return _mapper.Map<CustomProductViewModel>(customProduct);
		}

		// Admin, Staff xem tất cả custom product
		public async Task<List<CustomProductViewModel>> GetAllCustomProductsAsync()
		{
			var customProducts = await _unitOfWork.CustomProductRepository.GetAllAsync(cp => !cp.IsDeleted);
			return _mapper.Map<List<CustomProductViewModel>>(customProducts);
		}

		// Xoá mềm custom product
		public async Task<bool> DeleteCustomProductAsync(int customProductId)
		{
			var customProduct = await _unitOfWork.CustomProductRepository.GetByIdAsync(customProductId);
			if (customProduct == null) return false;

			customProduct.IsDeleted = true;
			await _unitOfWork.CustomProductRepository.Update(customProduct);
			await _unitOfWork.SaveChangesAsync();
			return true;
		}
		public async Task<CustomProductViewModel> AddCustomProductWithImageAsync(CustomProductUploadModel model)
		{
			var customProduct = new CustomProduct
			{
				ProductID = model.ProductID,
				CustomName = model.CustomName,
				Description = model.Description,
				Price = model.Price,
				ImageUrl = model.ImageUrl, // <-- dùng trực tiếp URL
				IsDeleted = false
			};

			await _unitOfWork.CustomProductRepository.AddAsync(customProduct);
			await _unitOfWork.SaveChangesAsync();

			return _mapper.Map<CustomProductViewModel>(customProduct);
		}

	}
}
