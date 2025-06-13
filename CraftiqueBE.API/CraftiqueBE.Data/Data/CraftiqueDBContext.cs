using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CraftiqueBE.Data.Data;
using CraftiqueBE.Data.Entities;
using CraftiqueBE.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CraftiqueBE.Data
{
    public class CraftiqueDBContext : IdentityDbContext<User>
	{
        public CraftiqueDBContext(DbContextOptions<CraftiqueDBContext> options) : base(options)
        {

        }
		public DbSet<Entities.Attribute> Attributes { get; set; }
		public DbSet<Blog> Blogs { get; set; }
		public DbSet<BlogImage> BlogImages { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<ChatMessage> ChatMessages { get; set; }
		public DbSet<ChatParticipant> ChatParticipants { get; set; }
		public DbSet<ChatRoom> ChatRooms { get; set; }
		public DbSet<Notification> Notifications { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderDetail> OrderDetails { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<ProductImg> ProductImgs { get; set; }
		public DbSet<ProductItem> ProductItems { get; set; }
		public DbSet<ProductItemAttribute> ProductItemAttributes { get; set; }
		public DbSet<Review> Reviews { get; set; }
		public DbSet<ShippingMethod> ShippingMethods { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<UserBlogView> UserBlogViews { get; set; }
		public DbSet<Voucher> Vouchers { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<Wallet> Wallets { get; set; }
		public DbSet<WalletTransaction> WalletTransactions { get; set; }
		public DbSet<Payment> Payments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			//insert role
			modelBuilder.ApplyConfiguration(new RoleConfiguration());

			// Configure composite key for ChatParticipant
			modelBuilder.Entity<ChatParticipant>()
				.HasKey(cp => new { cp.ChatRoomID, cp.UserID });

			// Configure User relationships
			modelBuilder.Entity<User>()
				.HasMany(u => u.ShippedOrders)
				.WithOne(o => o.Shipper)
				.HasForeignKey(o => o.ShipperID)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<User>()
				.HasMany(u => u.ShippedReviews)
				.WithOne(r => r.Shipper)
				.HasForeignKey(r => r.ShipperID)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure Order relationships with User
			modelBuilder.Entity<Order>()
				.HasOne(o => o.User)
				.WithMany(u => u.Orders)
				.HasForeignKey(o => o.UserID)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Order>()
				.HasOne(o => o.Shipper)
				.WithMany(u => u.ShippedOrders)
				.HasForeignKey(o => o.ShipperID)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure Review relationships with User
			modelBuilder.Entity<Review>()
				.HasOne(r => r.User)
				.WithMany(u => u.Reviews)
				.HasForeignKey(r => r.UserID)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Review>()
				.HasOne(r => r.Shipper)
				.WithMany(u => u.ShippedReviews)
				.HasForeignKey(r => r.ShipperID)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure ChatParticipant composite key
			modelBuilder.Entity<ChatParticipant>()
				.HasKey(cp => cp.ID);

			modelBuilder.Entity<ChatParticipant>()
				.HasOne(cp => cp.ChatRoom)
				.WithMany(c => c.ChatParticipants)
				.HasForeignKey(cp => cp.ChatRoomID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<ChatParticipant>()
				.HasOne(cp => cp.User)
				.WithMany(u => u.ChatParticipants)
				.HasForeignKey(cp => cp.UserID)
				.OnDelete(DeleteBehavior.Cascade);

			// Configure Voucher relationships
			modelBuilder.Entity<Voucher>()
				.HasOne(v => v.Product)
				.WithMany(p => p.Vouchers)
				.HasForeignKey(v => v.ProductID)
				.OnDelete(DeleteBehavior.Restrict);  // Change from Cascade to Restrict

			modelBuilder.Entity<Voucher>()
				.HasOne(v => v.ProductItem)
				.WithMany(pi => pi.Vouchers)
				.HasForeignKey(v => v.ProductItemID)
				.OnDelete(DeleteBehavior.Restrict);  // Change from Cascade to Restrict

			// Configure ProductItem relationship with Product
			modelBuilder.Entity<ProductItem>()
				.HasOne(pi => pi.Product)
				.WithMany(p => p.ProductItems)
				.HasForeignKey(pi => pi.ProductID)
				.OnDelete(DeleteBehavior.Cascade);

			// Other existing configurations...
			modelBuilder.Entity<Order>()
				.HasOne(o => o.User)
				.WithMany(u => u.Orders)
				.HasForeignKey(o => o.UserID)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Order>()
				.HasOne(o => o.Shipper)
				.WithMany(u => u.ShippedOrders)
				.HasForeignKey(o => o.ShipperID)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure Review relationships
			modelBuilder.Entity<Review>()
				.HasOne(r => r.ProductItem)
				.WithMany(pi => pi.Reviews)
				.HasForeignKey(r => r.ProductItemID)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Review>()
				.HasOne(r => r.OrderDetail)
				.WithMany(od => od.Reviews)
				.HasForeignKey(r => r.OrderDetailID)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Review>()
				.HasOne(r => r.User)
				.WithMany(u => u.Reviews)
				.HasForeignKey(r => r.UserID)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Review>()
				.HasOne(r => r.Shipper)
				.WithMany(u => u.ShippedReviews)
				.HasForeignKey(r => r.ShipperID)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure OrderDetail relationships
			modelBuilder.Entity<OrderDetail>()
				.HasOne(od => od.Order)
				.WithMany(o => o.OrderDetails)
				.HasForeignKey(od => od.OrderID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<OrderDetail>()
				.HasOne(od => od.ProductItem)
				.WithMany(pi => pi.OrderDetails)
				.HasForeignKey(od => od.ProductItemID)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure ProductItem relationship with Product
			modelBuilder.Entity<ProductItem>()
				.HasOne(pi => pi.Product)
				.WithMany(p => p.ProductItems)
				.HasForeignKey(pi => pi.ProductID)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure Voucher relationships (from previous fix)
			modelBuilder.Entity<Voucher>()
				.HasOne(v => v.Product)
				.WithMany(p => p.Vouchers)
				.HasForeignKey(v => v.ProductID)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Voucher>()
				.HasOne(v => v.ProductItem)
				.WithMany(pi => pi.Vouchers)
				.HasForeignKey(v => v.ProductItemID)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Payment>()
				.Property(p => p.Amount)
				.HasPrecision(18, 2);

			modelBuilder.Entity<Wallet>()
				.Property(w => w.Balance)
				.HasPrecision(18, 2);

			modelBuilder.Entity<WalletTransaction>()
				.Property(t => t.Amount)
				.HasPrecision(18, 2);
		}

	}


}
