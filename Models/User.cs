using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
    }
}