using Microsoft.EntityFrameworkCore;
using BusinessObject.Models;
using Microsoft.Extensions.Configuration;

namespace DataAccessLayer.DataContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

   
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<CitizenIdentityCard> CitizenIdentityCards { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ReputationEvent> ReputationEvents { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PayosTransaction> PayosTransactions { get; set; }


        private static string? GetConnectionString()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
            return configuration["ConnectionStrings:DefaultConnection"];
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(GetConnectionString());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Role)
                .WithMany(r => r.Accounts)
                .HasForeignKey(a => a.RoleId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.District)
                .WithMany(d => d.Accounts) 
                .HasForeignKey(a => a.DistrictId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Achievement)
                .WithMany(d => d.Accounts)
                .HasForeignKey(a => a.AchievementId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CitizenIdentityCard>()
                .HasOne(c => c.Account)
                .WithOne(a => a.CitizenIdentityCard)
                .HasForeignKey<CitizenIdentityCard>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Account)
                .WithMany(a => a.Subscriptions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Package)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(s => s.PackageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ward>()
                .HasOne(w => w.District)  
                .WithMany(d => d.Wards)   
                .HasForeignKey(w => w.DistrictId)  
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(a => a.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Payment)
                .WithOne(p => p.Subscription)
                .HasForeignKey<Payment>(p => p.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Payment>()
                .HasOne(p => p.PayosTransaction)
                .WithOne(pt => pt.Payment)
                .HasForeignKey<PayosTransaction>(pt => pt.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PayosTransaction>()
                .HasIndex(p => p.OrderCode)
                .IsUnique();

            modelBuilder.Entity<PayosTransaction>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("NOW()");

            modelBuilder.Entity<PayosTransaction>()
                .Property(p => p.UpdatedAt)
                .HasDefaultValueSql("NOW()");


            base.OnModelCreating(modelBuilder);
        }
    }
}
