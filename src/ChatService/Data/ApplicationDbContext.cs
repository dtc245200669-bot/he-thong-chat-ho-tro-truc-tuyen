using ChatService.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Conversation> Conversations => Set<Conversation>();

    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.CustomerName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(c => c.AgentName)
                .HasMaxLength(100);

            entity.Property(c => c.Status)
                .HasMaxLength(20)
                .IsRequired();

            entity.HasIndex(c => c.Status);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(m => m.Id);

            entity.Property(m => m.SenderName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(m => m.SenderType)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(m => m.Content)
                .HasMaxLength(5000)
                .IsRequired();

            entity.HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(m => new
            {
                m.ConversationId,
                m.SentAt
            });
        });
    }
}