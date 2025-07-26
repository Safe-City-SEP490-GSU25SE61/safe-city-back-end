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
        public DbSet<Commune> Communes { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ReputationEvent> ReputationEvents { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PayosTransaction> PayosTransactions { get; set; }
        public DbSet<PackageChangeHistory> PackageChanges { get; set; }
        public DbSet<AssignOfficerHistory> AssignOffers { get; set; }
        public DbSet<IncidentReport> IncidentReports { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<BlogLike> BlogLikes { get; set; }
        public DbSet<BlogMedia> BlogMedias { get; set; }
        public DbSet<BlogModeration> BlogModerations { get; set; }
        public DbSet<Province> Provinces { get; set; }




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
                .HasOne(a => a.Commune)
                .WithMany(d => d.Accounts) 
                .HasForeignKey(a => a.CommuneId)
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

           
            modelBuilder.Entity<IncidentReport>()
                .HasOne(ir => ir.User)
                .WithMany(a => a.IncidentReports)
                .HasForeignKey(ir => ir.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<IncidentReport>()
                .HasOne(ir => ir.Verifier)
                .WithMany(a => a.VerifiedIncidentReports)
                .HasForeignKey(ir => ir.VerifiedBy)
                .OnDelete(DeleteBehavior.SetNull);

            
            modelBuilder.Entity<IncidentReport>()
                .HasOne(ir => ir.Commune)
                .WithMany(d => d.IncidentReports)
                .HasForeignKey(ir => ir.CommuneId)
                .OnDelete(DeleteBehavior.Restrict);

   

            modelBuilder.Entity<Note>()
                .HasOne(n => n.Report)
                .WithMany(r => r.Notes)
                .HasForeignKey(n => n.ReportId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Note>()
                .HasOne(n => n.Officer)
                .WithMany(a => a.Notes)
                .HasForeignKey(n => n.OfficerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Blog>()
                .Property(b => b.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Blog>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Blogs)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Blog>()
                .HasOne(b => b.Commune)
                .WithMany(d => d.Blogs)
                .HasForeignKey(b => b.CommuneId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Blog>()
                .HasOne(b => b.Moderation)
                .WithOne(m => m.Blog)
                .HasForeignKey<BlogModeration>(m => m.BlogId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Blog)
                .WithMany(b => b.Comments)
                .HasForeignKey(c => c.BlogId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BlogLike>()
                .HasOne(pl => pl.User)
                .WithMany(a => a.BlogLikes)
                .HasForeignKey(pl => pl.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BlogLike>()
                .HasOne(pl => pl.Blog)
                .WithMany(b => b.Likes)
                .HasForeignKey(pl => pl.BlogId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BlogLike>()
                .HasIndex(pl => new { pl.UserId, pl.BlogId })
                .IsUnique();

            modelBuilder.Entity<BlogMedia>()
                .HasOne(m => m.Blog)
                .WithMany(b => b.Media)
                .HasForeignKey(m => m.BlogId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .Property(a => a.ReputationPoint)
                .HasDefaultValue(3);

            modelBuilder.Entity<IncidentReport>()
                .Property(r => r.Type)
                .HasConversion<string>();
            modelBuilder.Entity<IncidentReport>()
                .Property(r => r.TrafficSubCategory)
                .HasConversion<string>();
            modelBuilder.Entity<IncidentReport>()
                .Property(r => r.SecuritySubCategory)
                .HasConversion<string>();
            modelBuilder.Entity<IncidentReport>()
                .Property(r => r.InfrastructureSubCategory)
                .HasConversion<string>();
            modelBuilder.Entity<IncidentReport>()
                .Property(r => r.EnvironmentSubCategory)
                .HasConversion<string>();
            modelBuilder.Entity<IncidentReport>()
                .Property(r => r.OtherSubCategory)
                .HasConversion<string>();
            modelBuilder.Entity<IncidentReport>()
                .Property(r => r.PriorityLevel)
                .HasConversion<string>();

            modelBuilder.Entity<Commune>()
                .HasOne(c => c.Province)
                .WithMany(p => p.Communes)
                .HasForeignKey(c => c.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
