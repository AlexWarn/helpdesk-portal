namespace HelpDesk.Application.Users.Register;

public sealed record RegisterUserResult(
    Guid Id,
    string Email,
    string FullName);