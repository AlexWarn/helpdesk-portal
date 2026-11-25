namespace HelpDesk.Application.Exceptions;

public sealed class ConflictException(string code, string message)
    : Exception(message)
{
    public readonly string Code = code;
}