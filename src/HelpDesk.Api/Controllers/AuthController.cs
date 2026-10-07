using HelpDesk.Application.Users.Register;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Api.Controllers;

public sealed record RegisterUserRequest(
    string Email,
    string FirstName,
    string LastName,
    string? MiddleName,
    string Password);

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly RegisterUserUseCase _registerUserUseCase;

    public AuthController(
        RegisterUserUseCase registerUserUseCase)
    {
        _registerUserUseCase = registerUserUseCase;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.FirstName,
            request.LastName,
            request.MiddleName,
            request.Password);

        var result = await _registerUserUseCase.ExecuteAsync(
            command,
            cancellationToken);

        return Created(
            $"/api/users/{result.Id}",
            result);
    }
}