using CrudBankApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CrudBankApp.Data
{
    public class CrudBankAppDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }

        public CrudBankAppDbContext(DbContextOptions<CrudBankAppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // One-to-One relationship between IdentityUser and UserProfile
            modelBuilder.Entity<UserProfile>()
                .HasOne(up => up.IdentityUser)
                .WithOne()
                .HasForeignKey<UserProfile>(up => up.IdentityUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many relationship between UserProfile and Account
            modelBuilder.Entity<UserProfile>()
                .HasMany(up => up.Accounts)
                .WithOne(a => a.UserProfile)
                .HasForeignKey(a => a.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many relationship between AccountType and Account
            modelBuilder.Entity<AccountType>()
                .HasMany(at => at.Accounts)
                .WithOne(a => a.AccountType)
                .HasForeignKey(a => a.AccountTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Account properties
            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(a => a.Balance)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(a => a.Number)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(a => a.MinPay)
                    .HasPrecision(18, 2)
                    .HasDefaultValue(0);

                entity.HasIndex(a => a.Number)
                    .IsUnique();
            });

            // Configure AccountType properties
            modelBuilder.Entity<AccountType>(entity =>
            {
                entity.Property(at => at.Name)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(at => at.Description)
                    .HasMaxLength(200);

                entity.HasIndex(at => at.Name)
                    .IsUnique();
            });

            // Configure PaymentType properties
            modelBuilder.Entity<PaymentType>(entity =>
            {
                entity.Property(pt => pt.Name)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(pt => pt.Description)
                    .HasMaxLength(200);

                entity.HasIndex(pt => pt.Name)
                    .IsUnique();
            });

            // Seed AccountTypes
            modelBuilder.Entity<AccountType>().HasData(
                new AccountType 
                { 
                    Id = 1, 
                    Name = "Checking",
                    Description = "Standard checking account for daily transactions"
                },
                new AccountType 
                { 
                    Id = 2, 
                    Name = "Savings",
                    Description = "Interest-bearing savings account"
                },
                new AccountType 
                { 
                    Id = 3, 
                    Name = "Credit Card",
                    Description = "Revolving credit account with monthly payments"
                },
                new AccountType 
                { 
                    Id = 4, 
                    Name = "Business",
                    Description = "Business checking account with enhanced features"
                }
            );

            // Seed PaymentTypes
            modelBuilder.Entity<PaymentType>().HasData(
                new PaymentType 
                { 
                    Id = 1, 
                    Name = "Credit Card",
                    Description = "Payment using credit card"
                },
                new PaymentType 
                { 
                    Id = 2, 
                    Name = "Debit Card",
                    Description = "Direct payment from checking account"
                },
                new PaymentType 
                { 
                    Id = 3, 
                    Name = "Wire Transfer",
                    Description = "Electronic funds transfer between accounts"
                },
                new PaymentType 
                { 
                    Id = 4, 
                    Name = "ACH Transfer",
                    Description = "Automated Clearing House transfer"
                }
            );
        }
    }
}