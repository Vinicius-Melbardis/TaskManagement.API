using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs;

public class UpdateTaskDto
{
    [Required(ErrorMessage = "The title is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "The title must have between 3 and 100 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "The description can have a maximum of 500 characters.")]
    public string? Description { get; set; }

    public bool IsCompleted { get; set; }
}