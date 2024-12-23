using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TodoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TodoController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId() => int.Parse(User.FindFirst("id")?.Value ?? "0");

        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            var userId = GetUserId();
            var todos = await _context.TodoItems.Where(t => t.UserId == userId).ToListAsync();
            return Ok(todos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodo(TodoItem todo)
        {
            todo.UserId = GetUserId();
            _context.TodoItems.Add(todo);
            await _context.SaveChangesAsync();
            return Ok(todo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, TodoItem todo)
        {
            var userId = GetUserId();
            var existingTodo = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if(existingTodo == null)
                return NotFound();

            existingTodo.Title = todo.Title;
            existingTodo.IsCompleted = todo.IsCompleted;
            await _context.SaveChangesAsync();
            return Ok(existingTodo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var userId = GetUserId();
            var todo = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if(todo == null)
                return NotFound();

            _context.TodoItems.Remove(todo);
            await _context.SaveChangesAsync();
            return Ok("Eliminado");
        }
    }
}
