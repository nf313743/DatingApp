using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public sealed class DataContext :DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
        
    }

    public DbSet<AppUser> Users => Set<AppUser>();
}