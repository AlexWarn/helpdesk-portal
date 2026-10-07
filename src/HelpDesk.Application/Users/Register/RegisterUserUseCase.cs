using HelpDesk.Application.Abstractions;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.ValueObjects;

namespace HelpDesk.Application.Users.Register;

public sealed class RegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher; 
    private readonly IPasswordPolicy _passwordPolicy;
    private readonly IClock _clock;
    private readonly IUnitOfWork _unitOfWork;
    
    public RegisterUserUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IPasswordPolicy passwordPolicy,
        IClock clock,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _clock = clock;
        _unitOfWork = unitOfWork;
        _passwordPolicy = passwordPolicy;
    }

    public async Task<RegisterUserResult> ExecuteAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var email = Email.Create(command.Email);

        var fullName = FullName.Create(
            command.FirstName,
            command.LastName,
            command.MiddleName);

        var emailExists = await _userRepository.ExistsByEmailAsync(
            email,
            cancellationToken);

        if (emailExists)
        {
            throw new DomainException("User with this email already exists.");
        }
        
        _passwordPolicy.Validate(command.Password);
        
        var passwordHash = _passwordHasher
            .Hash(command.Password);
        
        var user = User.Create(
            email,
            passwordHash,
            fullName,
            _clock.UtcNow);
        
        await _userRepository.AddAsync(
            user,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
        
        return new RegisterUserResult(
            user.Id,
            user.Email.Value,
            user.FullName.Value);
    }
}