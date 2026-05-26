using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Services;
using TaskManager.Domain.Repositories;
using TaskManager.Infrastructure.Legacy;
using TaskManager.Infrastructure.Repositories;

namespace TaskManager.ConsoleApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();
                services.AddScoped<TaskService>();
                services.AddScoped<LegacyBatchImportService>();
            })
            .Build();

        var logger = host.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Task Manager Console App started");

        var taskService = host.Services.GetRequiredService<TaskService>();
        logger.LogInformation("TaskService resolved from DI container");

        var batchImport = host.Services.GetRequiredService<LegacyBatchImportService>();
        var records = new List<(int Id, string Data, int Type)>
        {
            (1, "Hello World Task", 1),
            (2, "ANOTHER TASK", 2),
            (3, "pass through", 3),
        };
        List<string> imported = batchImport.ImportBatch(records);
        logger.LogInformation("Batch import complete: {Count} records processed, last={Last}",
            batchImport.GetProcessedCount(), batchImport.GetLastProcessed());

        await host.RunAsync();
    }
}
