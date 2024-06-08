using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TodoApp.Models;

namespace TodoApp.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Look for any users.
                if (context.Users.Any())
                {
                    return;   // DB has been seeded
                }

                var users = new User[10];
                for (int i = 0; i < 10; i++)
                {
                    var user = new User
                    {
                        Username = $"user{i + 1}",
                        PasswordHash = CreatePasswordHash("password")
                    };
                    users[i] = user;
                    context.Users.Add(user);
                }

                context.SaveChanges();

                foreach (var user in users)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        context.TodoItems.Add(new TodoItem
                        {
                            Description = $"Todo {j + 1} for {user.Username}",
                            IsComplete = false,
                            UserId = user.Id,
                            User = user  // Ensure the User property is set
                        });
                    }
                }

                context.SaveChanges();
            }
        }

        private static string CreatePasswordHash(string password)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hashBytes = hmac.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}