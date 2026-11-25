namespace HelpDesk.Api.Contracts.Auth;

public sealed record RegisterUserRequest(
    string Email,
    string FirstName,
    string LastName,
    string? MiddleName,
    string Password);