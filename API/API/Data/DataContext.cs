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
    // 1-Many relation between Users and Photo (1 user can have many photos)
    // we would not need to create Db context for photos b/c we would not need CRUD on Photos
    
}