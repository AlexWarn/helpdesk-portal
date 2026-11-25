using System.Net;
using System.Net.Http.Json;
using HelpDesk.Api.Contracts.Auth;
using HelpDesk.Api.Contracts.Common;
using HelpDesk.Application.Abstractions;
using HelpDesk.Application.Constants;
using HelpDesk.Application.Users.Register;
using HelpDesk.Domain.Enums;
using HelpDesk.Domain.ValueObjects;
using HelpDesk.Infrastructure.Persistence;
using HelpDesk.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HelpDesk.IntegrationTests.Api.Auth;

public sealed class RegisterUserTests(HelpDeskWebApplicationFactory factory)
    : IClassFixture<HelpDeskWebApplicationFactory>,
      IAsyncLifetime
{
    public Task InitializeAsync()
    {
        return factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
    
    [Fact]
    public async Task Register_WithValidRequest_ShouldReturn201Created()
    {
        // Arrange
        using var client = factory.CreateClient();

        var request = new RegisterUserRequest(
            "  Alex.Test@Example.com  ",
            "Alex",
            "Warn",
            null,
            "Password1!");

        // Act
        using var response = await client.PostAsJsonAsync(
            "/api/auth/register",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var result = await response.Content
            .ReadFromJsonAsync<RegisterUserResult>();

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Warn Alex", result.FullName);

        // Verify database
        using var scope = factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var user = await dbContext.Users
            .AsNoTracking()
            .SingleAsync(x => x.Id == result.Id);

        Assert.Equal(result.Id, user.Id);

        const string expectedEmail = "alex.test@example.com";

        Assert.Equal(expectedEmail, result.Email);
        Assert.Equal(expectedEmail, user.Email.Value);
        
        Assert.Equal("Warn Alex", user.FullName.Value);
        Assert.Null(user.FullName.MiddleName);
        Assert.Equal(UserRole.Client, user.Role);
        Assert.False(user.IsBlocked);
        
        Assert.NotEqual(default, user.CreatedAtUtc);
        Assert.Equal(DateTimeKind.Utc, user.CreatedAtUtc.Kind);

        // Verify password hash
        Assert.NotEqual(request.Password, user.PasswordHash);

        var passwordHasher = scope.ServiceProvider
            .GetRequiredService<IPasswordHasher>();

        Assert.True(
            passwordHasher.Verify(
                request.Password,
                user.PasswordHash));
    }
    
    [Fact]
    public async Task Register_WithExistingEmail_ShouldReturn409Conflict()
    {
        // Arrange
        using var client = factory.CreateClient();

        var request = new RegisterUserRequest(
            "duplicate@example.com",
            "Alex",
            "Warn",
            null,
            "Password1!");

        using var firstResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            request);

        Assert.Equal(
            HttpStatusCode.Created,
            firstResponse.StatusCode);

        // Act
        using var response = await client.PostAsJsonAsync(
            "/api/auth/register",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Conflict,
            response.StatusCode);

        var result = await response.Content
            .ReadFromJsonAsync<ApiErrorResponse>();

        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Data);

        var error = Assert.Single(result.Errors);

        Assert.Equal(
            ErrorCodes.UserEmailAlreadyExists,
            error.Code);
        
        // Verify database
        using var scope = factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var email = Email.Create(request.Email);

        var usersCount = await dbContext.Users
            .CountAsync(x => x.Email == email);

        Assert.Equal(1, usersCount);
    }

    [Fact]
    public async Task Register_WithInvalidPassword_ShouldReturn400BadRequest()
    {
        // Arrange
        using var client = factory.CreateClient();

        var request = new RegisterUserRequest(
            "duplicate@example.com",
            "Alex",
            "Warn",
            null,
            "password");

        // Act
        using var response = await client.PostAsJsonAsync(
            "/api/auth/register",
            request);
        
        // Assert
        
        Assert.Equal(HttpStatusCode.BadRequest,  response.StatusCode);
        
        var result = await response.Content
            .ReadFromJsonAsync<ApiErrorResponse>();
        
        Assert.NotNull(result);
        Assert.Contains(result.Errors, x => x.Code == ErrorCodes.DomainError);
        
        // Verify database
        using var scope = factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var email = Email.Create(request.Email);
        
        var userExists = await dbContext.Users
            .AsNoTracking()
            .AnyAsync(x => x.Email == email);

        Assert.False(userExists);
    }

 
}