using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TodoApp.Models;
using TodoApp.Services;
using System;
using System.Linq;

namespace TodoApp.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            var userService = serviceProvider.GetRequiredService<IUserService>();

            for (int i = 1; i <= 10; i++)
            {
                var user = new User
                {
                    Username = $"User{i}"
                    // Add any additional fields here if necessary
                };

                userService.Register(user, "Password123").Wait();

                // Retrieve the registered user to ensure the User object is correctly populated
                var registeredUser = context.Users.SingleOrDefault(u => u.Username == user.Username);

                if (registeredUser != null)
                {
                    // Create 10 TodoItems for each user
                    for (int j = 1; j <= 10; j++)
                    {
                        var todoItem = new TodoItem
                        {
                            Description = $"Todo item {j} for {registeredUser.Username}",
                            IsComplete = false,
                            UserId = registeredUser.Id,  // Assuming UserId is the foreign key in TodoItem
                            User = registeredUser
                        };

                        context.TodoItems.Add(todoItem);
                    }
                }
            }

            context.SaveChanges();
        }
    }
}