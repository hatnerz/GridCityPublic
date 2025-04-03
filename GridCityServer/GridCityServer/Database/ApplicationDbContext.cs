using GridCityServer.Models;
using Microsoft.EntityFrameworkCore;

namespace GridCityServer.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<Player> Players { get; set; }
    public DbSet<MatchHistory> MatchesHistory { get; set; }
    public DbSet<MatchPlayer> MatchPlayers { get; set; }

    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MatchHistory>()
            .HasOne<Player>()
            .WithMany()
            .HasForeignKey(e => e.WinnerPlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MatchPlayer>()
            .HasOne<MatchHistory>()
            .WithMany(e => e.Players)
            .HasForeignKey(e => e.MatchId);

        modelBuilder.Entity<MatchPlayer>()
            .HasOne<Player>()
            .WithMany()
            .HasForeignKey(e => e.PlayerId);

        modelBuilder.Entity<MatchPlayer>().HasKey(e => e.Id);
    }
}
