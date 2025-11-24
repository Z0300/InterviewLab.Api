using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Problem> Problems => Set<Problem>();
    public DbSet<Solution> Solutions => Set<Solution>();
    public DbSet<User> Users => Set<User>();
}