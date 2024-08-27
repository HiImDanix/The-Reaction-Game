using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Domain;
using Domain.MiniGames;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure;

public class Repository: DbContext
{
    
    public Repository(DbContextOptions<Repository> options) : base(options)
    {
    }
    
    
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<MiniGame> MiniGames { get; set; }
    public DbSet<PlayerScore> PlayerScores { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MiniGame>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<ColorTapGame>(nameof(ColorTapGame));
        
        modelBuilder.Entity<ColorTapWordPairDisplay>()
            .Property(e => e.Color)
            .HasConversion(new ColorConverter());

        modelBuilder.Entity<ColorTapWordPairDisplay>()
            .Property(e => e.Word)
            .HasConversion(new ColorConverter());
    }
}

public class ColorConverter : ValueConverter<Color, string>
{
    public ColorConverter() : base(
        v => ColorToString(v),
        v => StringToColor(v))
    {}

    private static string ColorToString(Color color)
    {
        return $"{color.R},{color.G},{color.B},{color.A}";
    }

    private static Color StringToColor(string value)
    {
        var parts = value.Split(',');
        return Color.FromArgb(
            int.Parse(parts[3]), 
            int.Parse(parts[0]), 
            int.Parse(parts[1]), 
            int.Parse(parts[2]));
    }
}