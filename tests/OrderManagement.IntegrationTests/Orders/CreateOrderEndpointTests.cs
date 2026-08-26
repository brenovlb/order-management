using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using OrderManagement.IntegrationTests.Infrastructure;

namespace OrderManagement.IntegrationTests.Orders;

public sealed class CreateOrderEndpointTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CreateOrderEndpointTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnCreated_WhenRequestIsValid()
    {
        HttpResponseMessage loginResponse =
            await _client.PostAsJsonAsync(
                "/auth/login",
                new
                {
                    email = "dev@martech.com",
                    password = "Senha@123"
                });

        LoginResponse? login =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(login);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken);

        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/orders",
                new
                {
                    customerId = Guid.NewGuid(),
                    items = new[]
                    {
                        new
                        {
                            productName = "Notebook",
                            quantity = 2,
                            unitPrice = 3500m
                        }
                    }
                });

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnUnauthorized_WhenTokenIsMissing()
    {
        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/orders",
                new
                {
                    customerId = Guid.NewGuid(),
                    items = new[]
                    {
                    new
                    {
                        productName = "Notebook",
                        quantity = 1,
                        unitPrice = 3500m
                    }
                    }
                });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnBadRequest_WhenQuantityIsInvalid()
    {
        HttpResponseMessage loginResponse =
            await _client.PostAsJsonAsync(
                "/auth/login",
                new
                {
                    email = "dev@martech.com",
                    password = "Senha@123"
                });

        LoginResponse? login =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(login);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken);

        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/orders",
                new
                {
                    customerId = Guid.NewGuid(),
                    items = new[]
                    {
                    new
                    {
                        productName = "Notebook",
                        quantity = 0,
                        unitPrice = 3500m
                    }
                    }
                });

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    private sealed record LoginResponse(
        string AccessToken);
}