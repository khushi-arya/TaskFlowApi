using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Repositories;

namespace TaskManager.Tests.Integration;

/// <summary>
/// Integration tests for TaskRepository using in-memory database
/// </summary>
public class TaskRepositoryIntegrationTests : IDisposable
{
    private readonly TaskManagerDbContext _context;
    private readonly TaskRepository _repository;

    public TaskRepositoryIntegrationTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TaskManagerDbContext(options);
        _repository = new TaskRepository(_context);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WithMultipleTasks_ReturnsAllTasks()
    {
        // Arrange
        var tasks = new List<Task>
        {
            new Task { Title = "Task 1", Description = "Description 1", Priority = 1, Status = TaskStatus.Pending },
            new Task { Title = "Task 2", Description = "Description 2", Priority = 2, Status = TaskStatus.InProgress },
            new Task { Title = "Task 3", Description = "Description 3", Priority = 3, Status = TaskStatus.Completed }
        };

        foreach (var task in tasks)
        {
            _context.Tasks.Add(task);
        }
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(t => t.Title == "Task 1");
        result.Should().Contain(t => t.Title == "Task 2");
        result.Should().Contain(t => t.Title == "Task 3");
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyDatabase_ReturnsEmptyCollection()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_OrdersByCreatedAtDescending()
    {
        // Arrange
        var task1 = new Task { Title = "Task 1", CreatedAt = DateTime.UtcNow.AddHours(-2) };
        var task2 = new Task { Title = "Task 2", CreatedAt = DateTime.UtcNow.AddHours(-1) };
        var task3 = new Task { Title = "Task 3", CreatedAt = DateTime.UtcNow };

        _context.Tasks.Add(task1);
        _context.Tasks.Add(task2);
        _context.Tasks.Add(task3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert - Most recent first
        result.First().Title.Should().Be("Task 3");
        result.Last().Title.Should().Be("Task 1");
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsTask()
    {
        // Arrange
        var task = new Task { Title = "Test Task", Description = "Test Description", Priority = 2 };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var taskId = task.Id;

        // Act
        var result = await _repository.GetByIdAsync(taskId);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Task");
        result.Description.Should().Be("Test Description");
        result.Priority.Should().Be(2);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidTask_CreatesAndReturnsTask()
    {
        // Arrange
        var task = new Task
        {
            Title = "New Task",
            Description = "New Description",
            Priority = 3,
            Status = TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.CreateAsync(task);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0); // Id should be auto-generated
        result.Title.Should().Be("New Task");

        // Verify it's actually in the database
        var retrievedTask = await _repository.GetByIdAsync(result.Id);
        retrievedTask.Should().NotBeNull();
        retrievedTask!.Title.Should().Be("New Task");
    }

    [Fact]
    public async Task CreateAsync_GeneratesUniqueIds()
    {
        // Arrange
        var task1 = new Task { Title = "Task 1" };
        var task2 = new Task { Title = "Task 2" };

        // Act
        var created1 = await _repository.CreateAsync(task1);
        var created2 = await _repository.CreateAsync(task2);

        // Assert
        created1.Id.Should().NotBe(created2.Id);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidTask_UpdatesTask()
    {
        // Arrange
        var task = new Task
        {
            Title = "Original Title",
            Description = "Original Description",
            Priority = 1,
            Status = TaskStatus.Pending
        };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Modify the task
        task.Title = "Updated Title";
        task.Description = "Updated Description";
        task.Priority = 5;
        task.Status = TaskStatus.InProgress;

        // Act
        var result = await _repository.UpdateAsync(task);

        // Assert
        result.Title.Should().Be("Updated Title");
        result.Description.Should().Be("Updated Description");
        result.Priority.Should().Be(5);
        result.Status.Should().Be(TaskStatus.InProgress);

        // Verify it's updated in the database
        var retrievedTask = await _repository.GetByIdAsync(task.Id);
        retrievedTask!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task UpdateAsync_WithCompletionData_UpdatesStatusAndCompletedAt()
    {
        // Arrange
        var task = new Task
        {
            Title = "Task to Complete",
            Status = TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var completedTime = DateTime.UtcNow;
        task.Status = TaskStatus.Completed;
        task.CompletedAt = completedTime;

        // Act
        var result = await _repository.UpdateAsync(task);

        // Assert
        result.Status.Should().Be(TaskStatus.Completed);
        result.CompletedAt.Should().NotBeNull();
        result.CompletedAt!.Value.Should().BeCloseTo(completedTime, TimeSpan.FromSeconds(1));
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesTask()
    {
        // Arrange
        var task = new Task { Title = "Task to Delete" };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var taskId = task.Id;

        // Act
        var result = await _repository.DeleteAsync(taskId);

        // Assert
        result.Should().BeTrue();

        // Verify it's deleted from the database
        var retrievedTask = await _repository.GetByIdAsync(taskId);
        retrievedTask.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        // Act
        var result = await _repository.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WithMultipleTasks_DeletesOnlySpecificTask()
    {
        // Arrange
        var task1 = new Task { Title = "Task 1" };
        var task2 = new Task { Title = "Task 2" };
        var task3 = new Task { Title = "Task 3" };

        _context.Tasks.AddRange(task1, task2, task3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(task2.Id);

        // Assert
        result.Should().BeTrue();

        var allTasks = await _repository.GetAllAsync();
        allTasks.Should().HaveCount(2);
        allTasks.Should().Contain(t => t.Id == task1.Id);
        allTasks.Should().Contain(t => t.Id == task3.Id);
        allTasks.Should().NotContain(t => t.Id == task2.Id);
    }

    #endregion

    #region Complex Scenarios Tests

    [Fact]
    public async Task CompleteWorkflow_CreateUpdateDelete_Works()
    {
        // Arrange & Act - Create
        var task = new Task
        {
            Title = "Workflow Task",
            Description = "Testing complete workflow",
            Priority = 2,
            Status = TaskStatus.Pending
        };

        var createdTask = await _repository.CreateAsync(task);
        createdTask.Should().NotBeNull();

        // Act - Retrieve
        var retrievedTask = await _repository.GetByIdAsync(createdTask.Id);
        retrievedTask.Should().NotBeNull();
        retrievedTask!.Title.Should().Be("Workflow Task");

        // Act - Update
        retrievedTask.Status = TaskStatus.InProgress;
        retrievedTask.Priority = 4;
        var updatedTask = await _repository.UpdateAsync(retrievedTask);
        updatedTask.Priority.Should().Be(4);

        // Act - Delete
        var deleteResult = await _repository.DeleteAsync(updatedTask.Id);
        deleteResult.Should().BeTrue();

        // Assert - Verify deleted
        var finalTask = await _repository.GetByIdAsync(updatedTask.Id);
        finalTask.Should().BeNull();
    }

    [Fact]
    public async Task MultipleOperations_MaintainsDataIntegrity()
    {
        // Arrange - Create multiple tasks
        var tasks = Enumerable.Range(1, 5)
            .Select(i => new Task { Title = $"Task {i}", Priority = i })
            .ToList();

        foreach (var task in tasks)
        {
            await _repository.CreateAsync(task);
        }

        // Act - Retrieve all
        var allTasks = await _repository.GetAllAsync();

        // Assert
        allTasks.Should().HaveCount(5);

        // Act - Update one
        var taskToUpdate = allTasks.First();
        taskToUpdate.Title = "Updated Task";
        await _repository.UpdateAsync(taskToUpdate);

        // Assert - Verify count unchanged
        var allTasksAfterUpdate = await _repository.GetAllAsync();
        allTasksAfterUpdate.Should().HaveCount(5);

        // Act - Delete one
        await _repository.DeleteAsync(taskToUpdate.Id);

        // Assert - Verify count decreased
        var allTasksAfterDelete = await _repository.GetAllAsync();
        allTasksAfterDelete.Should().HaveCount(4);
    }

    #endregion
}
