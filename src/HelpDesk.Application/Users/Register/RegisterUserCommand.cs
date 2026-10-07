namespace HelpDesk.Application.Users.Register;

public sealed record RegisterUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string? MiddleName,
    string Password);