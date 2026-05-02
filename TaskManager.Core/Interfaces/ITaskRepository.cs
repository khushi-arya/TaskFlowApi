namespace TaskManager.Core.Interfaces;

using TaskManager.Core.Entities;

/// <summary>
/// Repository interface for Task entity
/// </summary>
public interface ITaskRepository
{
    Task<Task?> GetByIdAsync(int id);
    Task<IEnumerable<Task>> GetAllAsync();
    Task<Task> CreateAsync(Task task);
    Task<Task> UpdateAsync(Task task);
    Task<bool> DeleteAsync(int id);
}
