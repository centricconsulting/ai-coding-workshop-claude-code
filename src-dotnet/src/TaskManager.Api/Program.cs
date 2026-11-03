using TaskManager.Api.Extensions;

namespace TaskManager.Api;

/// <summary>
/// Task Manager API, the exercise substrate for the workshop labs
/// 
/// This API is organized using extension methods for:
/// - Service registration (Clean Architecture dependencies)
/// - OpenAPI documentation configuration
/// - Endpoint mapping and handlers
/// - Logging configuration
/// - OpenTelemetry observability (tracing to console)
/// 
/// ENDPOINTS (EndpointExtensions):
/// - POST /tasks is implemented in Lab 3, Part 2 (Requirements → Code)
/// - GET /tasks/{id}, PUT /tasks/{id}/status, and GET /tasks are extension work
///   (Lab 3 extension, Lab 5 integration tests)
/// 
/// OBSERVABILITY:
/// The API includes OpenTelemetry tracing that outputs to console, giving developers
/// exposure to modern observability practices and distributed tracing concepts.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure services using extension methods
        builder.Services
            .AddApplicationServices()         // Clean Architecture dependencies
            .AddApplicationLogging()          // Logging configuration
            .AddOpenApiDocumentation()        // OpenAPI/Swagger setup
            .AddOpenTelemetryObservability(); // OpenTelemetry tracing & observability

        var app = builder.Build();

        // Configure middleware pipeline using extension methods
        app.UseHttpsRedirection();
        app.UseApplicationLogging();       // Request/response logging
        app.UseOpenApiDocumentation();     // OpenAPI middleware
        app.MapTaskEndpoints();            // API endpoints

        app.Run();
    }
}
