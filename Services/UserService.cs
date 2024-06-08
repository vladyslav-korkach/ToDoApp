using System;
using System.Linq;
using TodoApp.Models;
using TodoApp.Data;

namespace TodoApp.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext context;

        public UserService(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public User? Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var user = context.Users.SingleOrDefault(x => x.Username == username);

            if (user == null || !VerifyPasswordHash(password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }

        public Task<User> Register(User user, string password)
        {
            throw new NotImplementedException();
        }

        Task<User> IUserService.Authenticate(string username, string password)
        {
            throw new NotImplementedException();
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password), "Password cannot be null or empty");
            }

            if (string.IsNullOrEmpty(storedHash))
            {
                throw new ArgumentNullException(nameof(storedHash), "Stored hash cannot be null or empty");
            }

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return storedHash == Convert.ToBase64String(hash);
            }
        }
    }
}