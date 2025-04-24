using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalesOrderApp.Models;
using Microsoft.EntityFrameworkCore;

namespace SalesOrderApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<COM_CUSTOMER> COM_CUSTOMER { get; set; }
        public DbSet<SO_ITEM> SO_ITEM { get; set; }
        public DbSet<SO_ORDER> SO_ORDER { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // COM_CUSTOMER
            modelBuilder.Entity<COM_CUSTOMER>(entity =>
            {
                entity.ToTable("COM_CUSTOMER");
                entity.HasKey(e => e.COM_CUSTOMER_ID);
                entity.Property(e => e.COM_CUSTOMER_ID).ValueGeneratedOnAdd();
                entity.Property(e => e.CUSTOMER_NAME).HasMaxLength(100);
            });

            // SO_ORDER
            modelBuilder.Entity<SO_ORDER>(entity =>
            {
                entity.ToTable("SO_ORDER");
                entity.HasKey(e => e.SO_ORDER_ID);
                entity.Property(e => e.SO_ORDER_ID).ValueGeneratedOnAdd();
                entity.Property(e => e.ORDER_NO).HasMaxLength(20).HasDefaultValue("");
                entity.Property(e => e.ORDER_DATE).HasDefaultValue(new DateTime(1900, 1, 1));
                entity.Property(e => e.COM_CUSTOMER_ID).HasDefaultValue(-99);
                entity.Property(e => e.ADDRESS).HasMaxLength(100).HasDefaultValue("");

                entity.HasMany(e => e.Items)
                      .WithOne()
                      .HasForeignKey(i => i.SO_ORDER_ID)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(o => o.Customer)
                    .WithMany()
                    .HasForeignKey(o => o.COM_CUSTOMER_ID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // SO_ITEM
            modelBuilder.Entity<SO_ITEM>(entity =>
            {
                entity.ToTable("SO_ITEM");
                entity.HasKey(e => e.SO_ITEM_ID);
                entity.Property(e => e.SO_ITEM_ID).ValueGeneratedOnAdd();
                entity.Property(e => e.ITEM_NAME).HasMaxLength(100).HasDefaultValue("");
                entity.Property(e => e.QUANTITY).HasDefaultValue(-99);
                entity.Property(e => e.PRICE).HasDefaultValue(0.0);
                entity.Property(e => e.SO_ORDER_ID).HasDefaultValue(-99);
            });
        }
    }
}