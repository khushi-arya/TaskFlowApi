using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.DTOs;
using TaskManager.Core.Interfaces;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService taskService, ILogger<TasksController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    /// <summary>
    /// Get all tasks
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetAllTasks()
    {
        _logger.LogInformation("Getting all tasks");
        var tasks = await _taskService.GetAllTasksAsync();
        return Ok(tasks);
    }

    /// <summary>
    /// Get a specific task by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskResponseDto>> GetTaskById(int id)
    {
        _logger.LogInformation("Getting task with ID: {TaskId}", id);
        var task = await _taskService.GetTaskByIdAsync(id);
        
        if (task == null)
            return NotFound(new { message = $"Task with ID {id} not found" });

        return Ok(task);
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TaskResponseDto>> CreateTask([FromBody] CreateTaskDto createTaskDto)
    {
        _logger.LogInformation("Creating new task: {TaskTitle}", createTaskDto.Title);
        
        var task = await _taskService.CreateTaskAsync(createTaskDto);
        return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
    }

    /// <summary>
    /// Update an existing task
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
    {
        _logger.LogInformation("Updating task with ID: {TaskId}", id);
        
        try
        {
            var task = await _taskService.UpdateTaskAsync(id, updateTaskDto);
            return Ok(task);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Task with ID {id} not found" });
        }
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        _logger.LogInformation("Deleting task with ID: {TaskId}", id);
        
        var result = await _taskService.DeleteTaskAsync(id);
        if (!result)
            return NotFound(new { message = $"Task with ID {id} not found" });

        return NoContent();
    }

    /// <summary>
    /// Mark a task as completed
    /// </summary>
    [HttpPost("{id}/complete")]
    public async Task<ActionResult<TaskResponseDto>> CompleteTask(int id)
    {
        _logger.LogInformation("Completing task with ID: {TaskId}", id);
        
        try
        {
            await _taskService.CompleteTaskAsync(id);
            var task = await _taskService.GetTaskByIdAsync(id);
            return Ok(task);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Task with ID {id} not found" });
        }
    }
}
