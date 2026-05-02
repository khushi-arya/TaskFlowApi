namespace TaskManager.Core.Interfaces;

using TaskManager.Core.DTOs;

/// <summary>
/// Service interface for Task business logic
/// </summary>
public interface ITaskService
{
    Task<TaskResponseDto?> GetTaskByIdAsync(int id);
    Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync();
    Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto createTaskDto);
    Task<TaskResponseDto> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto);
    Task<bool> DeleteTaskAsync(int id);
    Task<bool> CompleteTaskAsync(int id);
}
