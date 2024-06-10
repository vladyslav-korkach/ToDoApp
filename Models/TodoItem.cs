using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
    public class TodoItem
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        public bool IsComplete { get; set; }

        public int UserId { get; set; }  // Foreign key

        public User? User { get; set; }   // Make nullable
    }
}