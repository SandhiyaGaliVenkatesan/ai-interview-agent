using InterviewAgent.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InterviewAgent.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<InterviewSession> Sessions => Set<InterviewSession>();
    public DbSet<InterviewTurn> Turns => Set<InterviewTurn>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InterviewSession>()
            .HasMany(s => s.Turns)
            .WithOne(t => t.Session!)
            .HasForeignKey(t => t.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InterviewTurn>()
            .Property(t => t.QuestionJson)
            .HasColumnType("TEXT");

        modelBuilder.Entity<InterviewTurn>()
            .Property(t => t.EvaluationJson)
            .HasColumnType("TEXT");
    }
}
