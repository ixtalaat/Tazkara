using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Tazkara.Application.DTOs.Auth;
using Xunit;

namespace Tazkara.Application.Tests;

public class ApiIntegrationTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(ApiTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AdminCategoryEndpoint_WhenAnonymous_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/categories", new { name = "Integration Test" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task OrganizerEventEndpoint_WhenAnonymous_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/events", new { title = "Integration Test" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
