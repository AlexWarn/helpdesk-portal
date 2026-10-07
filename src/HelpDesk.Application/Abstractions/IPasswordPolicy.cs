namespace HelpDesk.Application.Abstractions;

public interface IPasswordPolicy
{
    void Validate(string? password);
}