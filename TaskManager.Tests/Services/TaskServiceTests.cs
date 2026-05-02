using Xunit;
using Moq;
using FluentAssertions;
using TaskManager.Application.Services;
using TaskManager.Core.DTOs;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Tests.Services;

/// <summary>
/// Unit tests for TaskService class
/// </summary>
public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _mockRepository;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _mockRepository = new Mock<ITaskRepository>();
        _taskService = new TaskService(_mockRepository.Object);
    }

    #region GetAllTasksAsync Tests

    [Fact]
    public async Task GetAllTasksAsync_WithValidTasks_ReturnsAllTasks()
    {
        // Arrange
        var tasks = new List<Task>
        {
            new Task { Id = 1, Title = "Task 1", Description = "Desc 1", Priority = 1, Status = TaskStatus.Pending, CreatedAt = DateTime.UtcNow },
            new Task { Id = 2, Title = "Task 2", Description = "Desc 2", Priority = 2, Status = TaskStatus.InProgress, CreatedAt = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

        // Act
        var result = await _taskService.GetAllTasksAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Title.Should().Be("Task 1");
        result.Last().Title.Should().Be("Task 2");
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllTasksAsync_WithEmptyList_ReturnsEmptyCollection()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Task>());

        // Act
        var result = await _taskService.GetAllTasksAsync();

        // Assert
        result.Should().BeEmpty();
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    #endregion

    #region GetTaskByIdAsync Tests

    [Fact]
    public async Task GetTaskByIdAsync_WithValidId_ReturnsTask()
    {
        // Arrange
        var taskId = 1;
        var task = new Task
        {
            Id = taskId,
            Title = "Test Task",
            Description = "Test Description",
            Priority = 3,
            Status = TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(task);

        // Act
        var result = await _taskService.GetTaskByIdAsync(taskId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(taskId);
        result.Title.Should().Be("Test Task");
        result.Priority.Should().Be(3);
        _mockRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
    }

    [Fact]
    public async Task GetTaskByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var taskId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync((Task?)null);

        // Act
        var result = await _taskService.GetTaskByIdAsync(taskId);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
    }

    #endregion

    #region CreateTaskAsync Tests

    [Fact]
    public async Task CreateTaskAsync_WithValidDto_CreatesTask()
    {
        // Arrange
        var createDto = new CreateTaskDto
        {
            Title = "New Task",
            Description = "New Description",
            Priority = 2,
            DueDate = DateTime.UtcNow.AddDays(5)
        };

        var createdTask = new Task
        {
            Id = 1,
            Title = createDto.Title,
            Description = createDto.Description,
            Priority = createDto.Priority,
            DueDate = createDto.DueDate,
            Status = TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Task>())).ReturnsAsync(createdTask);

        // Act
        var result = await _taskService.CreateTaskAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Task");
        result.Priority.Should().Be(2);
        result.Status.Should().Be("Pending");
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Task>()), Times.Once);
    }

    [Fact]
    public async Task CreateTaskAsync_WithNullDescription_CreatesTaskSuccessfully()
    {
        // Arrange
        var createDto = new CreateTaskDto
        {
            Title = "Task Without Description",
            Description = null,
            Priority = 1,
            DueDate = null
        };

        var createdTask = new Task
        {
            Id = 1,
            Title = createDto.Title,
            Description = null,
            Priority = 1,
            Status = TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Task>())).ReturnsAsync(createdTask);

        // Act
        var result = await _taskService.CreateTaskAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Task Without Description");
        result.Description.Should().BeNull();
    }

    #endregion

    #region UpdateTaskAsync Tests

    [Fact]
    public async Task UpdateTaskAsync_WithValidId_UpdatesTask()
    {
        // Arrange
        var taskId = 1;
        var updateDto = new UpdateTaskDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Priority = 4
        };

        var existingTask = new Task
        {
            Id = taskId,
            Title = "Old Title",
            Description = "Old Description",
            Priority = 1,
            Status = TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var updatedTask = new Task
        {
            Id = taskId,
            Title = updateDto.Title,
            Description = updateDto.Description,
            Priority = updateDto.Priority ?? 1,
            Status = TaskStatus.Pending,
            CreatedAt = existingTask.CreatedAt
        };

        _mockRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(existingTask);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Task>())).ReturnsAsync(updatedTask);

        // Act
        var result = await _taskService.UpdateTaskAsync(taskId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Updated Title");
        result.Description.Should().Be("Updated Description");
        result.Priority.Should().Be(4);
        _mockRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Task>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var taskId = 999;
        var updateDto = new UpdateTaskDto { Title = "Updated Title" };

        _mockRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync((Task?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _taskService.UpdateTaskAsync(taskId, updateDto));
        _mockRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Task>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTaskAsync_WithPartialUpdate_UpdatesOnlyProvidedFields()
    {
        // Arrange
        var taskId = 1;
        var updateDto = new UpdateTaskDto
        {
            Title = "Only Title Updated",
            Description = null,
            Priority = null,
            DueDate = null
        };

        var existingTask = new Task
        {
            Id = taskId,
            Title = "Original Title",
            Description = "Original Description",
            Priority = 2,
            Status = TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(existingTask);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Task>())).ReturnsAsync(existingTask);

        // Act
        var result = await _taskService.UpdateTaskAsync(taskId, updateDto);

        // Assert
        result.Title.Should().Be("Only Title Updated");
    }

    #endregion

    #region DeleteTaskAsync Tests

    [Fact]
    public async Task DeleteTaskAsync_WithValidId_DeletesTaskSuccessfully()
    {
        // Arrange
        var taskId = 1;
        _mockRepository.Setup(r => r.DeleteAsync(taskId)).ReturnsAsync(true);

        // Act
        var result = await _taskService.DeleteTaskAsync(taskId);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.DeleteAsync(taskId), Times.Once);
    }

    [Fact]
    public async Task DeleteTaskAsync_WithInvalidId_ReturnsFalse()
    {
        // Arrange
        var taskId = 999;
        _mockRepository.Setup(r => r.DeleteAsync(taskId)).ReturnsAsync(false);

        // Act
        var result = await _taskService.DeleteTaskAsync(taskId);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(r => r.DeleteAsync(taskId), Times.Once);
    }

    #endregion

    #region CompleteTaskAsync Tests

    [Fact]
    public async Task CompleteTaskAsync_WithValidId_CompletesTask()
    {
        // Arrange
        var taskId = 1;
        var task = new Task
        {
            Id = taskId,
            Title = "Task to Complete",
            Status = TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(task);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Task>())).ReturnsAsync(task);

        // Act
        var result = await _taskService.CompleteTaskAsync(taskId);

        // Assert
        result.Should().BeTrue();
        task.Status.Should().Be(TaskStatus.Completed);
        task.CompletedAt.Should().NotBeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Task>()), Times.Once);
    }

    [Fact]
    public async Task CompleteTaskAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var taskId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync((Task?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _taskService.CompleteTaskAsync(taskId));
        _mockRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Task>()), Times.Never);
    }

    #endregion
}
