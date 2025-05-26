using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Data
{
	public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
	{
		public void Configure(EntityTypeBuilder<IdentityRole> builder)
		{
			// Tạo các Role mặc định
			builder.HasData(
				new IdentityRole
				{
					Id = "e5df0673-2801-41c1-924c-8a84ee3aaa70",
					Name = "Admin",
					NormalizedName = "ADMIN"
				},
			new IdentityRole
			{
				Id = "b1a290ef-4467-4339-9931-218d8aeb46c0",
				Name = "Staff",
				NormalizedName = "STAFF"
			},
			new IdentityRole
			{
				Id = "e450c7fa-cb9b-40b0-b5f2-7ad083eefd63",
				Name = "Customer",
				NormalizedName = "CUSTOMER"
			},
			new IdentityRole
			{
				Id = "f7c2d1b8-1234-4e56-7890-abcdef123456",
				Name = "Shipper",
				NormalizedName = "SHIPPER"
			}
			);
		}
	}
}
