using Messenger.Models.db;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Models;

public partial class MessengerDbContext : DbContext
{
    public MessengerDbContext()
    {
        
    }

    public MessengerDbContext(DbContextOptions<MessengerDbContext> options) : base(options)
    {
        
    }
    
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Chat> Chats { get; set; }
    public virtual DbSet<Message> Messages { get; set; }
    public virtual DbSet<ChatParticipant> ChatParticipants { get; set; }
    public virtual DbSet<MessageStatus> MessageStatuses { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string sqlServerConnectionEnv = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION") ??
                                     throw new ApplicationException("Environment variable SQLSERVER_CONNECTION is not set!");
            optionsBuilder.UseSqlServer(sqlServerConnectionEnv!);
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MessengerDbContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }
    
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}