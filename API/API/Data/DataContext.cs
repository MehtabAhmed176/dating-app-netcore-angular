using api.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext :DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }
    // create a users table in the DB
    // column would be extracted from AppUser Entity
    public DbSet<AppUser> Users { get; set; }
}