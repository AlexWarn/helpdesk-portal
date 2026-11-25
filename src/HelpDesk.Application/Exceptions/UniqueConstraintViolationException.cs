namespace HelpDesk.Application.Exceptions;

public sealed class UniqueConstraintViolationException(
    UniqueConstraint constraint,
    Exception? innerException = null)
    : Exception("A unique constraint was violated.", innerException)
{
    public UniqueConstraint Constraint { get; } = constraint;
}