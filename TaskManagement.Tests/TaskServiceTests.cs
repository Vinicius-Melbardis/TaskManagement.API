using System;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.Entities;
using TaskManagement.API.Services;
namespace TaskManagement.Tests;

public class TaskServiceTests
{
    [Fact]
    public void Update_ShouldReturnFalse_WhenTaskDoesNotExist()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var service = new TaskService(context);

        var updatedTask = new TaskItem
        {
            Title = "Updated title",
            Description = "Updated description",
            IsCompleted = true
        };

        var result = service.Update(999, updatedTask);

        Assert.False(result);
    }

    [Fact]
    public void Update_ShouldReturnTrue_WhenTaskExists()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var task = new TaskItem
        {
            Title = "Original title",
            Description = "Original description",
            IsCompleted = true
        };

        context.Tasks.Add(task);
        context.SaveChanges();

        var service = new TaskService(context);
        var updatedTask = new TaskItem
        {
            Title = "Updated title",
            Description = "Updated description",
            IsCompleted = true
        };

        var result = service.Update(task.Id, updatedTask);
        var savedTask = context.Tasks.Find(task.Id);

        Assert.True(result);
        Assert.NotNull(savedTask);
        Assert.Equal("Updated title", savedTask!.Title);
        Assert.Equal("Updated description", savedTask.Description);
        Assert.True(savedTask.IsCompleted);
    }

    [Fact]
    public void Update_ShouldThrowException_WhenTitleIsEmpty()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        using var context = new AppDbContext(options);
        var task = new TaskItem
        {
            Title = "Original title",
            Description = "Original description",
            IsCompleted = false
        };

        context.Tasks.Add(task);
        context.SaveChanges();

        var service = new TaskService(context);
        var updatedTask = new TaskItem
        {
            Title = "",
            Description = "Updated description",
            IsCompleted = true
        };

        Action act = () => service.Update(task.Id, updatedTask);

        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Equal("Title is required. (Parameter 'Title')", exception.Message);
    }

    [Fact]
    public void Create_ShouldCreateTask_WhenDataIsValid()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

        using var context = new AppDbContext(options);
        var service = new TaskService(context);

        var task = new TaskItem
        {
            Title = "New task",
            Description = "Task description",
            IsCompleted = true
        };

        var result = service.Create(task);
        var savedTask = context.Tasks.Find(result.Id);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("New task", result.Title);
        Assert.Equal("Task description", result.Description);
        Assert.False(result.IsCompleted);
        Assert.True(result.CreatedAt <= DateTime.UtcNow);
        Assert.NotNull(savedTask);
        Assert.False(savedTask!.IsCompleted);
    }

    [Fact] // Marca o método como um teste unitário executável pelo xUnit
    public void Create_ShouldThrowException_WhenTitleIsEmpty()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            using var context = new AppDbContext(options);
            var service = new TaskService(context);

            var task = new TaskItem
            {
                Title = "",
                Description = "Task description",
                IsCompleted = false
            };

            Action act = () => service.Create(task);

            var exception = Assert.Throws<ArgumentException>(act);
            Assert.Equal("Title is required. (Parameter 'Title')", exception.Message);
    }

    [Fact]
    public void Delete_ShouldReturnFalse_WhenTaskDoesNotExist()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var service = new TaskService(context);

        var result = service.Delete(999);

        Assert.False(result);
    }

    [Fact]
    public void Delete_ShouldReturnTrue_WhenTaskExists()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var task = new TaskItem
        {
            Title = "Task to delete",
            Description = "Task description",
            IsCompleted = false
        };

        context.Tasks.Add(task);
        context.SaveChanges();

        var service = new TaskService(context);
        var result = service.Delete(task.Id);
        var deletedTask = context.Tasks.Find(task.Id);

        Assert.True(result);
        Assert.Null(deletedTask);
    }

    [Fact]
    public void GetById_ShouldReturnNull_WhenTaskDoesNotExist()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var service = new TaskService(context);

        var result = service.GetById(999);

        Assert.Null(result);
    }

    [Fact]
    public void GetAll_ShouldReturnAllTasks()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        context.Tasks.AddRange(
            new TaskItem
            {
                Title = "Task 1",
                Description = "Description 1",
                IsCompleted = false
            },
            new TaskItem
            {
                Title = "Task 2",
                Description = "Description 2",
                IsCompleted = true
            }
        );
        context.SaveChanges();

        var service = new TaskService(context);
        var result = service.GetAll();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, task => task.Title == "Task 1");
        Assert.Contains(result, task => task.Title == "Task 2");
    }
}