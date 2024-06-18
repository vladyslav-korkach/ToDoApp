using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Users.Any())
                {
                    return;   // DB has been seeded
                }

                // Add roles
                var roles = new[]
                {
                    new Role { Name = "Admin" },
                    new Role { Name = "User" }
                };

                context.Roles.AddRange(roles);
                context.SaveChanges();

                // Add users
                var userService = serviceProvider.GetRequiredService<IUserService>();
                var users = new[]
                {
                    new User { Username = "admin" },
                    new User { Username = "user1" },
                    new User { Username = "user2" },
                    new User { Username = "user3" },
                    new User { Username = "user4" },
                    new User { Username = "user5" },
                    new User { Username = "user6" },
                    new User { Username = "user7" },
                    new User { Username = "user8" },
                    new User { Username = "user9" }
                };

                foreach (var user in users)
                {
                    userService.Register(user, "password").Wait();
                }

                // Assign roles
                var adminUser = context.Users.SingleOrDefault(u => u.Username == "admin");
                if (adminUser != null)
                {
                    userService.AssignRole(adminUser.Id, "Admin").Wait();
                }

                for (int i = 1; i < users.Length; i++)
                {
                    var user = context.Users.SingleOrDefault(u => u.Username == "user" + i);
                    if (user != null)
                    {
                        userService.AssignRole(user.Id, "User").Wait();
                    }
                }

                // Add TodoItems
                foreach (var user in context.Users.Include(u => u.TodoItems).ToList())
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        user.TodoItems.Add(new TodoItem { Description = $"Todo {i} for {user.Username}", User = user });
                    }
                }

                context.SaveChanges();
            }
        }
    }
}