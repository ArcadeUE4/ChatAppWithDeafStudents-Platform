using ChatAppWithDeafStudents.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatAppWithDeafStudents.API
{
    /// <summary>
    /// Represents the database context for the Chat Application.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Gets or sets the users DbSet.
        /// </summary>
        public DbSet<Users> Users { get; set; }

        /// <summary>
        /// Gets or sets the chats DbSet.
        /// </summary>
        public DbSet<Chats> Chats { get; set; }

        /// <summary>
        /// Gets or sets the chat members DbSet.
        /// </summary>
        public DbSet<ChatMembers> ChatMembers { get; set; }

        /// <summary>
        /// Gets or sets the messages DbSet.
        /// </summary>
        public DbSet<Messages> Messages { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to configure the context.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        /// <summary>
        /// Configures the model for the context. This is called when the context is first created 
        /// to set up the relationships, keys, and entity configurations.
        /// </summary>
        /// <param name="modelBuilder">The builder used to configure the model for the context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure composite primary key for ChatMembers
            modelBuilder.Entity<ChatMembers>()
                .HasKey(cm => new { cm.ChatId, cm.UserId });

            // Configure foreign key relationships
            modelBuilder.Entity<Messages>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId);

            modelBuilder.Entity<Messages>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.SenderId);

            // Add indexes for performance optimization
            // Index for retrieving messages by chat ID sorted by creation time
            modelBuilder.Entity<Messages>()
                .HasIndex(m => new { m.ChatId, m.CreatedAt })
                .IsDescending(false, true); // Descending on CreatedAt for efficient latest message queries

            // Index for finding messages by sender
            modelBuilder.Entity<Messages>()
                .HasIndex(m => m.SenderId);

            // Index for ChatMembers lookups
            modelBuilder.Entity<ChatMembers>()
                .HasIndex(cm => cm.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
