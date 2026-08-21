using HelpDesk.Domain.Exceptions;

namespace HelpDesk.Domain.ValueObjects;

public sealed record FullName
{
    public const int MaxLength = 256;
    
    public string FirstName { get; }

    public string LastName { get; }

    public string? MiddleName { get; }

    private FullName(
        string firstName,
        string lastName,
        string? middleName)
    {
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
    }

    public string Value =>
        MiddleName is null
            ? $"{LastName} {FirstName}"
            : $"{LastName} {FirstName} {MiddleName}";

    public static FullName Create(
        string? firstName,
        string? lastName,
        string? middleName)
    {
        var normalizedFirstName = NormalizeRequiredPart(firstName);
        var normalizedLastName = NormalizeRequiredPart(lastName);
        var normalizedMiddleName = NormalizeOptionalPart(middleName);

        ValidateRequiredPart(normalizedFirstName, "First name");
        ValidateRequiredPart(normalizedLastName, "Last name");

        var fullName = new FullName(
            normalizedFirstName,
            normalizedLastName,
            normalizedMiddleName);

        if (fullName.Value.Length > MaxLength)
        {
            throw new DomainException(
                $"Full name must not exceed {MaxLength} characters.");
        }

        return fullName;
    }
    
    public override string ToString() => Value;
    

    private static string NormalizeRequiredPart(string? value)
    {
        return value?.Trim() ?? string.Empty;
    }

    private static string? NormalizeOptionalPart(string? value)
    {
        var normalized = value?.Trim();

        return string.IsNullOrWhiteSpace(normalized)
            ? null
            : normalized;
    }

    private static void ValidateRequiredPart(
        string value,
        string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{fieldName} is required.");
        }
    }
}