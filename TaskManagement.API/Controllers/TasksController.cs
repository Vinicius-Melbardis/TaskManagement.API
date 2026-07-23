using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.DTOs;
using TaskManagement.API.Entities;
using TaskManagement.API.Services;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/v1/tasks")]
public class TasksController : ControllerBase
{
    private readonly TaskService _taskService;

    public TasksController(TaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TaskResponseDto> Create(CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            Title = dto.Title ?? string.Empty,
            Description = dto.Description ?? string.Empty,
        };

        _taskService.Create(task); 

        var response = new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title ?? string.Empty,
            Description = task.Description ?? string.Empty,
            IsCompleted = task.IsCompleted,
            CreatedAt = task.CreatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, response);
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(List<TaskResponseDto>), StatusCodes.Status200OK)]
    
    public ActionResult<List<TaskResponseDto>> GetAll()
    {
        var tasks = _taskService.GetAll();
        
        var response = tasks.Select(task => new TaskResponseDto
        {
            Id = task.Id, 
            Title = task.Title ?? string.Empty,
            Description = task.Description ?? string.Empty,
            IsCompleted = task.IsCompleted,
            CreatedAt = task.CreatedAt 
        }).ToList(); 

        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TaskResponseDto> GetById(int id)
    {
        var task = _taskService.GetById(id);

        if (task is null)
        {
            return NotFound();
        }

        var response = new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title ?? string.Empty,
            Description = task.Description ?? string.Empty,
            IsCompleted = task.IsCompleted,
            CreatedAt = task.CreatedAt
        };

        return Ok(response);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Update(int id, UpdateTaskDto dto)
    {
        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = dto.IsCompleted
        };

        var updated = _taskService.Update(id, task);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var deleted = _taskService.Delete(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}