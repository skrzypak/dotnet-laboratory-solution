using lab_dotnet_task.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace lab_dotnet_task
{
    public class DatabaseContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DbSet<UserModel> Users { get; set; } = null!;
        public DbSet<MessageModel> Messages { get; set; } = null!;

        public DatabaseContext(IConfiguration configuration, DbContextOptions<DatabaseContext> options) : base(options)
        {
            Configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Dodatkowe ustawienia modeli z ktorych generowana jest baza danych

            modelBuilder.Entity<UserModel>().HasKey(x => x.Id);
            modelBuilder.Entity<UserModel>().Property(x => x.Id).HasConversion(new GuidToStringConverter()).IsRequired();

            modelBuilder.Entity<UserModel>().HasIndex(x => x.Username).IsUnique();

            modelBuilder.Entity<UserModel>().Property(x => x.Username).IsRequired();
            modelBuilder.Entity<UserModel>().Property(x => x.Password).IsRequired();

            // TODO: Skonfigurj utworzone encje
            modelBuilder.Entity<MessageModel>().HasKey(x => x.Id);
            modelBuilder.Entity<MessageModel>().Property(x => x.Id).ValueGeneratedOnAdd().IsRequired();

            modelBuilder.Entity<MessageModel>().Property(x => x.FromUserId).IsRequired();
            modelBuilder.Entity<MessageModel>().Property(x => x.ToUserId).IsRequired();

            modelBuilder.Entity<MessageModel>().ToTable("message");
            modelBuilder.Entity<MessageModel>().Property(p => p.Id).HasColumnName("message_id");
            modelBuilder.Entity<MessageModel>().Property(p => p.FromUserId).HasColumnName("message_from_user_id");
            modelBuilder.Entity<MessageModel>().Property(p => p.ToUserId).HasColumnName("message_to_user_id");
            modelBuilder.Entity<MessageModel>().Property(p => p.Text).HasColumnName("message_text");

            modelBuilder.Entity<MessageModel>()
               .HasOne(m => m.FromUser)
               .WithMany(u => u.FromUserMessages)
               .HasForeignKey(m => m.FromUserId);

            modelBuilder.Entity<MessageModel>()
               .HasOne(m => m.ToUser)
               .WithMany(u => u.ToUserMessages)
               .HasForeignKey(m => m.ToUserId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Okreslenie polaczenia do bazy SQLite
            optionsBuilder.UseSqlite(Configuration.GetConnectionString("ApiDatabase"));

        }
    }
}
