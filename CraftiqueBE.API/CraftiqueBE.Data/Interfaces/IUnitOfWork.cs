using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Interfaces
{
	public interface IUnitOfWork : IDisposable
	{
		IRepository<Entities.Attribute> AttributeRepository { get; }
		IRepository<BlogImage> BlogImageRepository { get; }
		IRepository<Blog> BlogRepository { get; }
		IRepository<Category> CategoryRepository { get; }
		IRepository<ChatMessage> ChatMessageRepository { get; }
		IRepository<ChatParticipant> ChatParticipantRepository { get; }
		IRepository<ChatRoom> ChatRoomRepository { get; }
		IRepository<OrderDetail> OrderDetailRepository { get; }
		IRepository<Order> OrderRepository { get; }
		IRepository<ProductImg> ProductImgRepository { get; }
		IRepository<ProductItemAttribute> ProductItemAttributeRepository { get; }
		IRepository<ProductItem> ProductItemRepository { get; }
		IRepository<Product> ProductRepository { get; }
		IRepository<Review> ReviewRepository { get; }
		IRepository<User> UserRepository { get; }
		IRepository<Entities.Notification> NotificationRepository { get; }
		IRepository<Payment> PaymentRepository { get; }
		IRepository<PaymentTransaction> TransactionRepository { get; }
		IRepository<WorkshopRegistration> WorkshopRegistrationRepository { get; }
		IRepository<CustomProduct> CustomProductRepository { get; }
		IRepository<CustomProductFile> CustomProductFileRepository { get; }

		Task BeginTransactionAsync();
		Task CommitTransactionAsync();
		void Dispose();
		Task RollbackTransactionAsync();
		Task<int> SaveChangesAsync();
	}
}
