using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApp.Models
{
    public class TodoItem
    {
        public int Id { get; set; }

        [Required]
        public required string Description { get; set; }

        public bool IsComplete { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public required User User { get; set; }
    }
}