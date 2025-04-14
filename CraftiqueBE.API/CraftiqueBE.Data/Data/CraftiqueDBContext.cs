using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CraftiqueBE.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CraftiqueBE.Data
{
    public class CraftiqueDBContext : DbContext
    {
        public CraftiqueDBContext(DbContextOptions<CraftiqueDBContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(10, 2);
    }
    
    
}
