using Microsoft.EntityFrameworkCore;
using Domain;

namespace Infrastructure;

public class DbContext: Microsoft.EntityFrameworkCore.DbContext
{
    public DbContext(DbContextOptions<DbContext> options) : base(options)
    {
    }
    
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Player> Players { get; set; }
}