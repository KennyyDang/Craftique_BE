using AutoMapper;
using CraftiqueBE.Data.Interfaces;
using CraftiqueBE.Data.Models.AttributeModel;
using CraftiqueBE.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Service.Services
{
	public class AttributeServices : IAttributeServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public AttributeServices(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<IEnumerable<Data.Entities.Attribute>> GetAllAsync()
		{
			var result = await _unitOfWork.AttributeRepository.GetAllAsync();

			result = result?.Where(r => r.IsDeleted == false) ?? new List<Data.Entities.Attribute>();

			return result;
		}

		public async Task<Data.Entities.Attribute> GetByIdAsync(int id)
		{
			var attribute = await _unitOfWork.AttributeRepository.GetByIdAsync(id);
			if (attribute == null)
				throw new KeyNotFoundException("Attribute not found");

			return attribute;
		}

		public async Task<Data.Entities.Attribute> AddAsync(Data.Entities.Attribute attribute)
		{
			await _unitOfWork.BeginTransactionAsync();

			try
			{
				var result = await _unitOfWork.AttributeRepository.AddAsync(attribute);

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return result;
			}
			catch (Exception)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}

		public async Task<Data.Entities.Attribute> UpdateAsync(int id, UpdateAttributeModel newAttribute)
		{
			await _unitOfWork.BeginTransactionAsync();
			try
			{
				var attribute = await _unitOfWork.AttributeRepository.GetByIdAsync(id);
				if (attribute == null)
					throw new KeyNotFoundException($"Attribute with ID {id} not found.");

				bool IsInvalid(string value) => string.IsNullOrWhiteSpace(value) || value == "string";

				attribute.AttributeName = !IsInvalid(newAttribute.AttributeName) ? newAttribute.AttributeName : attribute.AttributeName;
				attribute.DataType = !IsInvalid(newAttribute.DataType) ? newAttribute.DataType : attribute.DataType;

				var result = await _unitOfWork.AttributeRepository.Update(attribute);

				if (!result)
				{
					throw new InvalidOperationException("Failed to update attribute.");
				}

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();
				return attribute;
			}
			catch (Exception)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}



		public async Task<Data.Entities.Attribute> DeleteAsync(int id)
		{
			await _unitOfWork.BeginTransactionAsync();
			try
			{
				var attribute = await _unitOfWork.AttributeRepository.GetByIdAsync(id);
				if (attribute == null)
					throw new KeyNotFoundException($"Attribute with ID {id} not found.");

				var result = await _unitOfWork.AttributeRepository.SoftDelete(attribute);
				if (!result)
				{
					throw new InvalidOperationException("Failed to delete attribute.");
				}

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();
				return attribute;
			}
			catch (Exception)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw;
			}
		}
	}
}
