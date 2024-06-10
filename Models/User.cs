using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

        [Required]
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        // Add any new fields here
        // Example: public string Email { get; set; } = string.Empty;
    }
}