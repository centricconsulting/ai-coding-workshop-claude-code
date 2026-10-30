namespace TaskManager.IntegrationTests;

/// <summary>
/// Integration tests for Task Manager API — Lab 5
///
/// LAB 5 INSTRUCTIONS:
/// Use Claude Code to generate integration tests for the API endpoints.
/// Follow the characterization-test pattern in lab-05 before refactoring.
///
/// Example prompts:
/// - "Generate integration tests for POST /tasks endpoint"
/// - "Create integration test for GET /tasks/{id} endpoint"
/// - "Add integration tests with proper HTTP status codes"
/// </summary>
public class TaskApiIntegrationTests
{
    [Fact(Skip = "Lab 5: Generate this integration test with Claude Code — follow lab-05")]
    public System.Threading.Tasks.Task CreateTask_WithValidData_ShouldReturn201()
    {
        // TODO: Lab 5 — test the POST /tasks endpoint
        return System.Threading.Tasks.Task.CompletedTask;
    }

    [Fact(Skip = "Lab 5: Generate this integration test with Claude Code — follow lab-05")]
    public System.Threading.Tasks.Task GetTask_WithExistingId_ShouldReturn200()
    {
        // TODO: Lab 5 — test the GET /tasks/{id} endpoint
        return System.Threading.Tasks.Task.CompletedTask;
    }

    [Fact(Skip = "Lab 5: Generate this integration test with Claude Code — follow lab-05")]
    public System.Threading.Tasks.Task GetTask_WithNonExistentId_ShouldReturn404()
    {
        // TODO: Lab 5 — test the 404 case
        return System.Threading.Tasks.Task.CompletedTask;
    }

    [Fact(Skip = "Lab 5: Generate this integration test with Claude Code — follow lab-05")]
    public System.Threading.Tasks.Task UpdateTaskStatus_WithValidData_ShouldReturn200()
    {
        // TODO: Lab 5 — test the PUT /tasks/{id}/status endpoint
        return System.Threading.Tasks.Task.CompletedTask;
    }

    [Fact(Skip = "Lab 5: Generate this integration test with Claude Code — follow lab-05")]
    public System.Threading.Tasks.Task GetActiveTasks_ShouldReturn200WithTaskList()
    {
        // TODO: Lab 5 — test the GET /tasks endpoint
        return System.Threading.Tasks.Task.CompletedTask;
    }
}
