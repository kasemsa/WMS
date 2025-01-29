using Microsoft.EntityFrameworkCore;
using System.Drawing.Text;
using WarehouseManagementSystem.Helper;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.DataBase
{
    public class WarehouseDbContext : DbContext
    {
        public WarehouseDbContext()
        {
            
        }

        public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserToken> UserToken { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Commissary> Commissaries { get; set; }
        public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Commissary>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Customer>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<InvoiceItem>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Permission>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<PurchaseInvoice>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Role>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<RolePermission>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<SalesInvoice>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<UserPermission>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<UserRole>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<UserToken>().HasQueryFilter(p => !p.IsDeleted);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(connectionString: GlobalAttributes.sqlConfiguration.connectionString);
            }
        }
    }
}
