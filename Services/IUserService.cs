using System.Threading.Tasks;
using TodoApp.Models;

namespace TodoApp.Services
{
    public interface IUserService
    {
        Task<User?> Authenticate(string username, string password);
        Task<User> Register(User user, string password);
    }
}