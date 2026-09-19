using Anazetesis.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anazetesis.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.ToTable("Conversations");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ClientId).HasMaxLength(128).IsRequired();
            entity.Property(e => e.Title).HasMaxLength(256);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasIndex(e => e.ClientId);
            entity.HasIndex(e => e.UpdatedAt);
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.ToTable("Messages");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Role).HasMaxLength(32).IsRequired();
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.Conversation)
                  .WithMany(c => c.Messages)
                  .HasForeignKey(e => e.ConversationId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ConversationId);
            entity.HasIndex(e => new { e.ConversationId, e.CreatedAt });
        });
    }
}