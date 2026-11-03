using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TaskManager.Api;

namespace TaskManager.IntegrationTests;

/// <summary>
/// Working example of the WebApplicationFactory pattern. Use it as the model
/// for the Lab 5 stubs in UnitTest1.cs.
/// </summary>
public sealed class HealthEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetHealth_WhenApiIsRunning_ReturnsOkWithHealthyStatus()
    {
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/health", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        HealthResponse? body = await response.Content.ReadFromJsonAsync<HealthResponse>(TestContext.Current.CancellationToken);
        Assert.Equal("Healthy", body?.Status);
    }

    private sealed record HealthResponse(string Status, string Message);
}
