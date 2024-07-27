using Microsoft.EntityFrameworkCore;
using Domain;

namespace Infrastructure;

public class RoomDbContext: DbContext
{
    public RoomDbContext(DbContextOptions<RoomDbContext> options) : base(options)
    {
    }
    
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Player> Players { get; set; }
}