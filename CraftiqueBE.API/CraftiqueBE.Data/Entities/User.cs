using CraftiqueBE.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Entities
{
	public class User : IdentityUser, IBaseEntity
	{
		[Required(ErrorMessage = "User name is required.")]
		[MaxLength(255, ErrorMessage = "User name cannot exceed 255 characters.")]
		public string Name { get; set; }

		[MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
		public string? Address { get; set; }

		[MaxLength(500, ErrorMessage = "Avatar URL cannot exceed 500 characters.")]
		public string? Avatar { get; set; }
		public bool IsDeleted { get; set; }

		public ICollection<Order> Orders { get; set; }
		public ICollection<Order> ShippedOrders { get; set; } //Shipper
		public ICollection<Review> Reviews { get; set; }
		public ICollection<Review> ShippedReviews { get; set; } //Shipper
		public ICollection<Notification>? Notifications { get; set; }
		public ICollection<UserBlogView> UserBlogViews { get; set; }
		public ICollection<ChatParticipant> ChatParticipants { get; set; }
	}
}
