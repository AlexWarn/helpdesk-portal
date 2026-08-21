using HelpDesk.Domain.Exceptions;

namespace HelpDesk.Domain.ValueObjects;

public sealed record Email
{
    public string Value { get; }  
    
    private Email(string value)
    {
        Value = value;
    }
    
    public static Email Create(string? value)
    {
        var normalized = Normalize(value);
        
        Validate(normalized);
        
        return new Email(normalized);
    }

    private static void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        { 
            throw new DomainException("Email is required.");
        }
    }

    private static string Normalize(string? value)
    {
       return value?.Trim().ToLowerInvariant() ?? string.Empty;
    }

    public override string ToString() => Value;
}