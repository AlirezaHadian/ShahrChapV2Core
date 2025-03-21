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
using System.Text.RegularExpressions;
using ShahrChap.DataLayer.Entities.Product.Form;

namespace ShahrChap.DataLayer.Context
{
    public class ShahrChapContext : DbContext
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
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductGallery> ProductGalleries { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<FeatureValue> FeatureValues { get; set; }
        public DbSet<ProductFeature> ProductFeatures { get; set; }
        public DbSet<ProductFeatureValue> ProductFeatureValues { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServicePrice> ServicePrices { get; set; }
        public DbSet<DesignPrice> DesignPrices { get; set; }
        public DbSet<ProductForm> ProductForms { get; set; }
        public DbSet<FormInput> FormInputs { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<Role>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<UserAddress>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<Product>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<ProductGroup>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<ProductGroup>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<Feature>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<FeatureValue>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<Service>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<ProductForm>().HasQueryFilter(u => !u.IsDelete);

            modelBuilder.Entity<ProductPrice>()
                .HasIndex(pc => new { pc.ProductId, pc.Combination })
                .IsUnique();
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

            // Features Relationships
            // FeatureValue -> Feature (One-to-Many)
            modelBuilder.Entity<FeatureValue>()
                .HasOne(fv => fv.Feature)
                .WithMany(f => f.FeatureValues)
                .HasForeignKey(fv => fv.FeatureId)
                .OnDelete(DeleteBehavior.Cascade); // Keep cascading for Features -> FeatureValues

            // ProductFeatureValue Relationships
            modelBuilder.Entity<ProductFeatureValue>()
                .HasOne(pfv => pfv.Product)
                .WithMany(p => p.ProductFeatureValues)
                .HasForeignKey(pfv => pfv.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductFeatureValue>()
                .HasOne(pfv => pfv.Feature)
                .WithMany(p => p.ProductFeatureValues)
                .HasForeignKey(pfv => pfv.FeatureId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductFeatureValue>()
                .HasOne(pfv => pfv.FeatureValue)
                .WithMany(fv => fv.ProductFeatureValues)
                .HasForeignKey(pfv => pfv.FeatureValueId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete to avoid multiple paths

            modelBuilder.Entity<ServicePrice>()
                .HasOne(psp => psp.Service)
                .WithMany(ps => ps.ServicePrices)
                .HasForeignKey(psp => psp.ProductServiceId)
                .OnDelete(DeleteBehavior.Restrict);
            base.OnModelCreating(modelBuilder);
        }
    }
}