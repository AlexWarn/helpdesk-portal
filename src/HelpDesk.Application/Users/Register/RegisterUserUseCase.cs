using HelpDesk.Application.Abstractions;
using HelpDesk.Application.Constants;
using HelpDesk.Application.Exceptions;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.ValueObjects;

namespace HelpDesk.Application.Users.Register;

public sealed class RegisterUserUseCase(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IPasswordPolicy passwordPolicy,
    IClock clock,
    IUnitOfWork unitOfWork)
{
    public async Task<RegisterUserResult> ExecuteAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var email = Email.Create(command.Email);

        var fullName = FullName.Create(
            command.FirstName,
            command.LastName,
            command.MiddleName);

        var emailExists = await userRepository.ExistsByEmailAsync(
            email,
            cancellationToken);

        if (emailExists)
        {
            throw new ConflictException(
                ErrorCodes.UserEmailAlreadyExists,
                "User with this email already exists.");
        }
        
        passwordPolicy.Validate(command.Password);
        
        var passwordHash = passwordHasher
            .Hash(command.Password);
        
        var user = User.Create(
            email,
            passwordHash,
            fullName,
            clock.UtcNow);
        
        await userRepository.AddAsync(
            user,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);
        
        return new RegisterUserResult(
            user.Id,
            user.Email.Value,
            user.FullName.Value);
    }
}