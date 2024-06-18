using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.Models;

namespace TodoApp.Services
{
    public interface IUserService
    {
        Task<User?> Authenticate(string username, string password);
        Task<User> Register(User user, string password);
        Task AssignRole(int userId, string roleName);
        Task<List<string>> GetUserRoles(int userId);
        Task<bool> IsAdmin(int userId);
        Task<User?> GetById(int id);
    }
}