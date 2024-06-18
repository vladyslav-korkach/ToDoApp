using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;
using TodoApp.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public TodoItemsController(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: api/TodoItems
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTodoItems()
        {
            var todoItems = await _context.TodoItems
                .Include(t => t.User)
                .ToListAsync();

            var todoItemsDto = todoItems.Select(t => new TodoItemDto
            {
                Id = t.Id,
                Description = t.Description,
                IsCompleted = t.IsCompleted,
                CreatedAt = t.CreatedAt,
                CompletedAt = t.CompletedAt,
                UserId = t.User.Id,
                Username = t.User.Username
            }).ToList();

            return Ok(todoItemsDto);
        }

        // GET: api/TodoItems/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
        {
            var user = await GetUser();
            if (user == null)
            {
                return NotFound("User not found.");
            }
            
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            if (await _userService.IsAdmin(user.Id) || todoItem.User.Id == user.Id)
            {
                return todoItem;
            }

            return Forbid();
        }

        // PUT: api/TodoItems/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, TodoItemDto todoItemDto)
        {
            var user = await GetUser();
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var todoItem = await _context.TodoItems.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);
            if (todoItem == null)
            {
                return NotFound("Todo item not found.");
            }

            if (!await _userService.IsAdmin(user.Id) && todoItem.User.Id != user.Id)
            {
                return Forbid();
            }

            // Update the properties of the todoItem with the values from todoItemDto
            todoItem.Description = todoItemDto.Description;
            todoItem.IsCompleted = false;
            // Update other properties as needed

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
        }

        // POST: api/TodoItems
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(CreateTodoItemDto createTodoItemDto)
        {
            var user = await GetUser();
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var todoItem = new TodoItem
            {
                Description = createTodoItemDto.Description,
                CreatedAt = DateTime.UtcNow,                
                User = user
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/TodoItems/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            var user = await GetUser();
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            if (!await _userService.IsAdmin(user.Id) && todoItem.User.Id != user.Id)
            {
                return Forbid();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(int id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
        

        private async Task<User?> GetUser ()
        //Cannot implicitly convert type 'System.Threading.Tasks.Task<TodoApp.Models.User?>' to 'TodoApp.Models.User'
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return null;
            }

            var userId = int.Parse(userIdClaim.Value);
            var user = await _userService.GetById(userId);
            return user;
        }
    }

    public class CreateTodoItemDto
    {
        public string Description { get; set; } = string.Empty;
    }


    public class TodoItemDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}