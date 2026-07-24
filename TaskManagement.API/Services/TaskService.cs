using TaskManagement.API.Data;
using TaskManagement.API.Entities;
namespace TaskManagement.API.Services;
public class TaskService
{
    private readonly AppDbContext _context;
    public TaskService(AppDbContext context)
    {
        _context = context;
    }
    public TaskItem Create(TaskItem task)
    {
        if (string.IsNullOrWhiteSpace(task.Title))
        {
            throw new ArgumentException("Title is required.", nameof(task.Title));
        }
        task.CreatedAt = DateTime.UtcNow;
        task.IsCompleted = false;

        _context.Tasks.Add(task);
        _context.SaveChanges();

        return task;
    }

    public List<TaskItem> GetAll()
    {
        return _context.Tasks.ToList();
    }

    public TaskItem? GetById(int id)
    {
        return _context.Tasks.Find(id);
    }

    public bool Update(int id, TaskItem updatedTask)
    {
        var existingTask = _context.Tasks.Find(id);

        if(existingTask is null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(updatedTask.Title))
        {
            throw new ArgumentException("Title is required.", nameof(updatedTask.Title));
        }

        existingTask.Title = updatedTask.Title;
        existingTask.Description = updatedTask.Description;
        existingTask.IsCompleted = updatedTask.IsCompleted;

        _context.SaveChanges();

        return true;
    }

    public bool Delete(int id)
    {
        var existingTask = _context.Tasks.Find(id);

        if (existingTask is null)
        {
            return false;
        }

        _context.Tasks.Remove(existingTask);
        _context.SaveChanges();
        
        return true;
    }
}