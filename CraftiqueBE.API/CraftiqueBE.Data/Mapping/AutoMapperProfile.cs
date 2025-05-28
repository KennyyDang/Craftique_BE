using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Models.AttributeModel;
using CraftiqueBE.Data.Models.AuthenticationModel;
using CraftiqueBE.Data.Models.BlogImageModel;
using CraftiqueBE.Data.Models.BlogModel;
using CraftiqueBE.Data.Models.CategoryModel;
using CraftiqueBE.Data.Models.ProductImgModel;
using CraftiqueBE.Data.Models.ProductItemAttributeModel;
using CraftiqueBE.Data.Models.ProductItemModel;
using CraftiqueBE.Data.Models.ProductModel;
using CraftiqueBE.Data.Models.ReviewModel;
using CraftiqueBE.Data.Models.UserModel;
using CraftiqueBE.Data.Models;
using CraftiqueBE.Data.ViewModels.AttributeVM;
using CraftiqueBE.Data.ViewModels.BlogVM;
using CraftiqueBE.Data.ViewModels.CategoryVM;
using CraftiqueBE.Data.ViewModels.ChatVM;
using CraftiqueBE.Data.ViewModels.OrderVM;
using CraftiqueBE.Data.ViewModels.ProductItemAttributeVM;
using CraftiqueBE.Data.ViewModels.ProductItemVM;
using CraftiqueBE.Data.ViewModels.ProductVM;
using CraftiqueBE.Data.ViewModels.ReviewVM;
using CraftiqueBE.Data.ViewModels.UserVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AutoMapper;

namespace CraftiqueBE.Data.Mapping
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<Category, CategoryViewModel>().ReverseMap();
			CreateMap<Category, CreateCategoryModel>().ReverseMap();
			CreateMap<Category, UpdateCategoryModel>().ReverseMap();

			CreateMap<Entities.Attribute, AttributeViewModel>().ReverseMap();
			CreateMap<Entities.Attribute, CreateAttributeModel>().ReverseMap();
			CreateMap<Entities.Attribute, UpdateAttributeModel>().ReverseMap();

			CreateMap<ProductItemAttribute, ProductItemAttributeViewModel>()
				.ForMember(dest => dest.AttributeName, opt => opt.MapFrom(src => src.Attribute != null ? src.Attribute.AttributeName : string.Empty));
			CreateMap<ProductItemAttribute, CreateProductItemAttributeModel>().ReverseMap();
			CreateMap<ProductItemAttribute, UpdateProductItemAttributeModel>().ReverseMap();

			// Review mappings
			CreateMap<Review, ReviewViewModel>()
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
				.ForMember(dest => dest.ShipperName, opt => opt.MapFrom(src => src.Shipper != null ? src.Shipper.Name : null));

			CreateMap<CreateReviewModel, Review>()
				.ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));

			CreateMap<Product, ProductViewModel>()
				.ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));
			CreateMap<CreateProductModel, Product>();
			CreateMap<Product, UpdateProductModel>().ReverseMap();

			CreateMap<ProductItem, ProductItemViewModel>()
				.ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
				.ForMember(dest => dest.ProductImgs, opt => opt.MapFrom(src => src.ProductImgs))
				.ForMember(dest => dest.ProductItemAttributes, opt => opt.MapFrom(src => src.ProductItemAttributes));

			CreateMap<CreateProductItemModel, ProductItem>()
				.ForMember(dest => dest.ProductImgs, opt => opt.MapFrom(src =>
					src.ImageUrls.Select(url => new ProductImg
					{
						ImageUrl = url,
						IsDeleted = false
					}).ToList()))
				.ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));

			CreateMap<UpdateProductItemModel, ProductItem>()
				.ForMember(dest => dest.ProductItemID, opt => opt.Ignore())
				.ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
				.ForMember(dest => dest.Product, opt => opt.Ignore())
				.ForMember(dest => dest.ProductImgs, opt => opt.Ignore())
				.ForMember(dest => dest.Reviews, opt => opt.Ignore())
				.ForMember(dest => dest.OrderDetails, opt => opt.Ignore())
				.ForMember(dest => dest.ProductItemAttributes, opt => opt.Ignore())
				.ForMember(dest => dest.Vouchers, opt => opt.Ignore());

			CreateMap<User, UserViewModel>()
				.ForMember(dest => dest.Role, opt => opt.Ignore())
				.ReverseMap(); // Role cần xử lý riêng
			CreateMap<UserModel, User>()
				.ForMember(dest => dest.UserName, opt => opt.Ignore())
				.ForMember(dest => dest.Email, opt => opt.Ignore())
				.ReverseMap();
			CreateMap<RegisterModel, User>()
			.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email)) // Map Email -> UserName
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
			.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
			.ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
			.ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
			.ReverseMap();

			CreateMap<ProductImg, CraftiqueBE.Data.ViewModels.ProductImgVM.ProductImgViewModel>().ReverseMap();
			CreateMap<CreateProductImgModel, ProductImg>()
				.ForMember(dest => dest.ProductImgID, opt => opt.Ignore())
				.ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));
			CreateMap<UpdateProductImgModel, ProductImg>()
				.ForMember(dest => dest.ProductImgID, opt => opt.Ignore())
				.ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
				.ForMember(dest => dest.ProductItemID, opt => opt.Ignore());

			CreateMap<Blog, BlogViewModel>();
			CreateMap<BlogImage, BlogImageViewModel>();
			CreateMap<CreateBlogModel, Blog>();
			CreateMap<CreateBlogImageModel, BlogImage>();

			CreateMap<ChatMessage, ChatMessageViewModel>()
				.ForMember(dest => dest.ChatID, opt => opt.MapFrom(src => src.ChatMessageID))
				.ForMember(dest => dest.SenderName, opt => opt.MapFrom(src =>
					src.User != null ? src.User.UserName : "Unknown User"));

			CreateMap<ChatRoom, ChatRoomViewModel>()
				.ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.ChatMessages))
				.ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.ChatParticipants))
				.ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src =>
					src.ChatMessages.OrderByDescending(m => m.CreatedDate).FirstOrDefault()));

			CreateMap<ChatParticipant, ChatParticipantViewModel>()
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src =>
					src.User != null ? src.User.UserName : "Unknown User"))
				.ForMember(dest => dest.IsOnline, opt => opt.Ignore());

			CreateMap<Order, OrderViewModel>()
				.ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));

			CreateMap<OrderDetail, OrderDetailViewModel>();

		}
	}
}
