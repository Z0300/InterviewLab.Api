using WebApi.Features.auth;
using WebApi.Models;

namespace WebApi.Utilities;

public static class DataSeeder
{
    public static async Task SeedDataAsync(AppDbContext context, PasswordHasher passwordHasher)
    {
        if (!context.Users.Any())
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "marv1n",
                PasswordHash = PasswordHasher.Hash("SecretKey3000!")
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
    }
}