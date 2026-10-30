using FakeItEasy;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Services;
using TaskManager.Domain.Repositories;
using TaskManager.Domain.Tasks;

namespace TaskManager.UnitTests;

/// <summary>
/// Unit tests for TaskService — Lab 3
///
/// LAB 3 (TDD) INSTRUCTIONS:
/// Use Claude Code to implement the TaskService.AddTaskAsync method and its tests.
/// Follow the TDD cycle: interface → tests → implementation.
///
/// LAB 3 (STRETCH) INSTRUCTIONS:
/// Use Claude Code to generate additional unit tests covering edge cases.
///
/// Example prompts:
/// - "Implement AddTaskAsync with validation and logging following src/CLAUDE.md conventions"
/// - "Generate unit tests for happy path and validation scenarios"
/// - "Add tests for null parameters and empty strings"
/// </summary>
public class TaskServiceTests
{
    private readonly ITaskRepository _mockRepository;
    private readonly ILogger<TaskService> _mockLogger;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _mockRepository = A.Fake<ITaskRepository>();
        _mockLogger = A.Fake<ILogger<TaskService>>();
        _taskService = new TaskService(_mockRepository, _mockLogger);
    }

    [Fact(Skip = "Lab 3: Implement this test with Claude Code — follow the TDD cycle in lab-03")]
    public System.Threading.Tasks.Task AddTaskAsync_WithValidData_ShouldReturnTaskId()
    {
        // Arrange
        // TODO: Set up test data and mock expectations

        // Act
        // TODO: Call TaskService.AddTaskAsync

        // Assert
        // TODO: Verify the result and mock interactions

        return System.Threading.Tasks.Task.CompletedTask;
    }

    [Fact(Skip = "Lab 3: Implement this test with Claude Code — follow the TDD cycle in lab-03")]
    public System.Threading.Tasks.Task AddTaskAsync_WithNullTitle_ShouldThrowArgumentException()
    {
        // TODO: Test validation for null title
        return System.Threading.Tasks.Task.CompletedTask;
    }

    [Fact(Skip = "Lab 3: Generate this test with Claude Code — follow the TDD cycle in lab-03")]
    public System.Threading.Tasks.Task AddTaskAsync_WithEmptyDescription_ShouldThrowArgumentException()
    {
        // TODO: Lab 3 — generate with Claude Code
        return System.Threading.Tasks.Task.CompletedTask;
    }

    [Fact(Skip = "Lab 3: Generate this test with Claude Code — follow the TDD cycle in lab-03")]
    public System.Threading.Tasks.Task GetTaskAsync_WithExistingId_ShouldReturnTask()
    {
        // TODO: Lab 3 — generate with Claude Code
        return System.Threading.Tasks.Task.CompletedTask;
    }

    [Fact(Skip = "Lab 3: Generate this test with Claude Code — follow the TDD cycle in lab-03")]
    public System.Threading.Tasks.Task GetTaskAsync_WithNonExistentId_ShouldReturnNull()
    {
        // TODO: Lab 3 — generate with Claude Code
        return System.Threading.Tasks.Task.CompletedTask;
    }

    [Fact(Skip = "Lab 3: Generate this test with Claude Code — follow the TDD cycle in lab-03")]
    public System.Threading.Tasks.Task UpdateTaskStatusAsync_WithValidData_ShouldUpdateSuccessfully()
    {
        // TODO: Lab 3 — generate with Claude Code
        return System.Threading.Tasks.Task.CompletedTask;
    }
}
