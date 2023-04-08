using Microsoft.EntityFrameworkCore;

namespace MonoAPI.Models;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
}