using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Repositories;

// Aliases to avoid ambiguity
using TaskEntity = TaskManager.Core.Entities.Task;
using TaskStatusEnum = TaskManager.Core.Entities.TaskStatus;

namespace TaskManager.Tests.Integration;
/// hello khushi arya 
/// <summary>
/// Integration tests for TaskRepository using in-memory database
/// </summary>
public class TaskRepositoryIntegrationTests : IDisposable
{
    private readonly TaskManagerDbContext _context;
    private readonly TaskRepository _repository;

    public TaskRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TaskManagerDbContext(options);
        _repository = new TaskRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WithMultipleTasks_ReturnsAllTasks()
    {
        var tasks = new List<TaskEntity>
        {
            new TaskEntity { Title = "Task 1", Description = "Description 1", Priority = 1, Status = TaskStatusEnum.Pending },
            new TaskEntity { Title = "Task 2", Description = "Description 2", Priority = 2, Status = TaskStatusEnum.InProgress },
            new TaskEntity { Title = "Task 3", Description = "Description 3", Priority = 3, Status = TaskStatusEnum.Completed }
        };

        await _context.Tasks.AddRangeAsync(tasks);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        result.Should().HaveCount(3);
        result.Should().Contain(t => t.Title == "Task 1");
        result.Should().Contain(t => t.Title == "Task 2");
        result.Should().Contain(t => t.Title == "Task 3");
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyDatabase_ReturnsEmptyCollection()
    {
        var result = await _repository.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_OrdersByCreatedAtDescending()
    {
        var task1 = new TaskEntity { Title = "Task 1", CreatedAt = DateTime.UtcNow.AddHours(-2) };
        var task2 = new TaskEntity { Title = "Task 2", CreatedAt = DateTime.UtcNow.AddHours(-1) };
        var task3 = new TaskEntity { Title = "Task 3", CreatedAt = DateTime.UtcNow };

        await _context.Tasks.AddRangeAsync(task1, task2, task3);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        result.First().Title.Should().Be("Task 3");
        result.Last().Title.Should().Be("Task 1");
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsTask()
    {
        var task = new TaskEntity
        {
            Title = "Test Task",
            Description = "Test Description",
            Priority = 2
        };

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(task.Id);

        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Task");
        result.Description.Should().Be("Test Description");
        result.Priority.Should().Be(2);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(999);

        result.Should().BeNull();
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidTask_CreatesAndReturnsTask()
    {
        var task = new TaskEntity
        {
            Title = "New Task",
            Description = "New Description",
            Priority = 3,
            Status = TaskStatusEnum.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _repository.CreateAsync(task);

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("New Task");

        var retrieved = await _repository.GetByIdAsync(result.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Title.Should().Be("New Task");
    }

    [Fact]
    public async Task CreateAsync_GeneratesUniqueIds()
    {
        var task1 = new TaskEntity { Title = "Task 1" };
        var task2 = new TaskEntity { Title = "Task 2" };

        var created1 = await _repository.CreateAsync(task1);
        var created2 = await _repository.CreateAsync(task2);

        created1.Id.Should().NotBe(created2.Id);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidTask_UpdatesTask()
    {
        var task = new TaskEntity
        {
            Title = "Original",
            Description = "Original Desc",
            Priority = 1,
            Status = TaskStatusEnum.Pending
        };

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        task.Title = "Updated";
        task.Description = "Updated Desc";
        task.Priority = 5;
        task.Status = TaskStatusEnum.InProgress;

        var result = await _repository.UpdateAsync(task);

        result.Title.Should().Be("Updated");
        result.Priority.Should().Be(5);
        result.Status.Should().Be(TaskStatusEnum.InProgress);

        var dbTask = await _repository.GetByIdAsync(task.Id);
        dbTask!.Title.Should().Be("Updated");
    }

    [Fact]
    public async Task UpdateAsync_WithCompletionData_UpdatesCompletedAt()
    {
        var task = new TaskEntity
        {
            Title = "Complete Me",
            Status = TaskStatusEnum.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        var completedTime = DateTime.UtcNow;
        task.Status = TaskStatusEnum.Completed;
        task.CompletedAt = completedTime;

        var result = await _repository.UpdateAsync(task);

        result.Status.Should().Be(TaskStatusEnum.Completed);
        result.CompletedAt.Should().NotBeNull();
        result.CompletedAt!.Value.Should().BeCloseTo(completedTime, TimeSpan.FromSeconds(1));
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesTask()
    {
        var task = new TaskEntity { Title = "Delete Me" };

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        var result = await _repository.DeleteAsync(task.Id);

        result.Should().BeTrue();

        var dbTask = await _repository.GetByIdAsync(task.Id);
        dbTask.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        var result = await _repository.DeleteAsync(999);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_DeletesOnlySpecifiedTask()
    {
        var t1 = new TaskEntity { Title = "Task 1" };
        var t2 = new TaskEntity { Title = "Task 2" };
        var t3 = new TaskEntity { Title = "Task 3" };

        await _context.Tasks.AddRangeAsync(t1, t2, t3);
        await _context.SaveChangesAsync();

        await _repository.DeleteAsync(t2.Id);

        var all = await _repository.GetAllAsync();

        all.Should().HaveCount(2);
        all.Should().Contain(x => x.Id == t1.Id);
        all.Should().Contain(x => x.Id == t3.Id);
        all.Should().NotContain(x => x.Id == t2.Id);
    }

    #endregion

    #region Workflow Tests

    [Fact]
    public async Task CompleteWorkflow_WorksCorrectly()
    {
        var task = new TaskEntity
        {
            Title = "Workflow",
            Priority = 2,
            Status = TaskStatusEnum.Pending
        };

        var created = await _repository.CreateAsync(task);

        var fetched = await _repository.GetByIdAsync(created.Id);
        fetched!.Status = TaskStatusEnum.InProgress;

        var updated = await _repository.UpdateAsync(fetched);

        var deleted = await _repository.DeleteAsync(updated.Id);

        deleted.Should().BeTrue();

        var final = await _repository.GetByIdAsync(updated.Id);
        final.Should().BeNull();
    }

    #endregion
}