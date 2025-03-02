using Microsoft.EntityFrameworkCore;

namespace TombalaAPI.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<GameCard> GameCards { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Game entity
            modelBuilder.Entity<Game>()
                .HasKey(g => g.Id);

            modelBuilder.Entity<Game>()
                .Property(g => g.Name)
                .IsRequired();

            modelBuilder.Entity<Game>()
                .HasMany(g => g.GameCards)
                .WithOne()
                .HasForeignKey(gc => gc.GameId);

            // Configure GameCard entity
            modelBuilder.Entity<GameCard>()
                .HasKey(gc => gc.Id);

            modelBuilder.Entity<GameCard>()
                .HasOne(gc => gc.User)
                .WithMany()
                .HasForeignKey("UserId");

            // Configure User entity
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Name)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.DiscordId)
                .IsUnique();

            // Configure many-to-many relationship between Game and User (Participants)
            modelBuilder.Entity<Game>()
                .HasMany(g => g.Participants)
                .WithMany()
                .UsingEntity(j => j.ToTable("GameParticipants"));

            // Configure JSON columns for lists
            modelBuilder.Entity<Game>()
                .Property(g => g.DrawnNumbers)
                .HasColumnType("jsonb");

            modelBuilder.Entity<GameCard>()
                .Property(gc => gc.Numbers)
                .HasColumnType("jsonb");

            modelBuilder.Entity<GameCard>()
                .Property(gc => gc.MarkedNumbers)
                .HasColumnType("jsonb");
        }
    }
} 