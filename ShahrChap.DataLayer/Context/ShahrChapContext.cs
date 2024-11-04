using Microsoft.EntityFrameworkCore;
using ShahrChap.DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShahrChap.DataLayer.Entities.Address;
using ShahrChap.DataLayer.Entities.Wallet;
using ShahrChap.DataLayer.Entities.Permissions;
using ShahrChap.DataLayer.Entities.Product;

namespace ShahrChap.DataLayer.Context
{
    public class ShahrChapContext:DbContext
    {
        public ShahrChapContext(DbContextOptions<ShahrChapContext> options) : base(options)
        {
            
        }

        #region User
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        #endregion
        #region Permission
        public DbSet<Permission> Permission { get; set; }
        public DbSet<RolePermission> RolePermission { get; set; }

        #endregion
        #region wallet
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletType> WalletTypes { get; set; }
        #endregion
        #region Address
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<City> City { get; set; }
        #endregion
        #region Product
        public DbSet<ProductGroup> ProductGroups { get; set; }
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<Role>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<UserAddress>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<ProductGroup>().HasQueryFilter(u => !u.IsDelete);

            // Province -> City (One-to-Many)
            modelBuilder.Entity<Province>()
                .HasMany(p => p.Cities)
                .WithOne(c => c.Province)
                .HasForeignKey(c => c.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Province -> UserAddress (One-to-Many)
            modelBuilder.Entity<Province>()
                .HasMany(p => p.UserAddresses)
                .WithOne(ua => ua.Province)
                .HasForeignKey(ua => ua.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // City -> UserAddress (One-to-Many)
            modelBuilder.Entity<City>()
                .HasMany(c => c.UserAddresses)
                .WithOne(ua => ua.City)
                .HasForeignKey(ua => ua.CityId)
                .OnDelete(DeleteBehavior.Cascade); // Allow cascade delete

            base.OnModelCreating(modelBuilder);
        }
    }
    
}
