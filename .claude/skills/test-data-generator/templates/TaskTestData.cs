// Template for the test-data-generator skill.
// Matches the starting domain model in src/TaskManager.Domain/Tasks.
// Adapt it when the model gains new members (priority, due date, assignee, ...).
using TaskManager.Domain.Tasks;
using Task = TaskManager.Domain.Tasks.Task;
using TaskStatus = TaskManager.Domain.Tasks.TaskStatus;

namespace TaskManager.UnitTests.TestData;

public static class TaskTestData
{
    public static Task ValidTask(
        string title = "Fix login timeout on mobile",
        string description = "Users are logged out after 30 seconds on iOS Safari.") =>
        Task.Create(title, description);

    public static Task TaskWithStatus(TaskStatus status)
    {
        var task = ValidTask();
        task.UpdateStatus(status);
        return task;
    }

    public static IReadOnlyList<Task> SampleTasks() =>
    [
        Task.Create("Write ADR for caching strategy", "Compare in-memory and distributed cache options."),
        Task.Create("Add pagination to task list endpoint", "Support page and pageSize query parameters."),
        TaskWithStatus(TaskStatus.InProgress),
        TaskWithStatus(TaskStatus.Done),
        TaskWithStatus(TaskStatus.Cancelled),
    ];

    // Edge cases: use with [Theory] + [MemberData] in validation tests.
    public static TheoryData<string> InvalidTitles() =>
    [
        "",
        "   ",
        "\t\n",
    ];

    public static TheoryData<string> UnusualButValidTitles() =>
    [
        "A",
        new string('x', 200),
        "Mise à jour du système 🚀",
        "タスクを完了する",
    ];

    public static TheoryData<TaskStatus> AllStatuses() =>
    [
        TaskStatus.Todo,
        TaskStatus.InProgress,
        TaskStatus.Done,
        TaskStatus.Cancelled,
    ];
}
