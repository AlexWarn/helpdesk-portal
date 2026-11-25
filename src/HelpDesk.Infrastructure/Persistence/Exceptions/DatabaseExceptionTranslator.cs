using HelpDesk.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HelpDesk.Infrastructure.Persistence.Exceptions;

internal static class DatabaseExceptionTranslator
{
    public static bool TryTranslate(
        DbUpdateException exception,
        out UniqueConstraintViolationException? translatedException)
    {
        translatedException = null;

        if (exception.InnerException is not PostgresException postgresException)
        {
            return false;
        }

        if (postgresException.SqlState != PostgresErrorCodes.UniqueViolation)
        {
            return false;
        }

        var constraint = postgresException.ConstraintName switch
        {
            "ix_users_email" => UniqueConstraint.UserEmail,
            _ => (UniqueConstraint?)null
        };

        if (constraint is null)
        {
            return false;
        }

        translatedException = new UniqueConstraintViolationException(
            constraint.Value,
            exception);

        return true;
    }
}