using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;

namespace HelpDesk.UnitTests.Application.Security;

public class PasswordPolicyTests
{
    private readonly PasswordPolicy _passwordPolicy = new();
    

    //[Fact]
    public void Validate_WithValidPassword_ShouldNotThrow()
    {
        // Arrange
        const string password = "1Password!";
        
        // Act
        _passwordPolicy.Validate(password);
    }

    //[Fact]
    public void Validate_WithTooShortPassword_ShouldThrowDomainException()
    {
        // Arrange
        const string password = "1Pass";
        
        // Act
        var action = () => _passwordPolicy.Validate(password);

        // Assert
        var exception = Assert.Throws<DomainException>(action);
        
        Assert.Equal("Password is too short.", exception.Message);
    }

    //[Fact]
    public void Validate_WithoutUppercaseLetter_ShouldThrowDomainException()
    {
        // Arrange
        const string password = "password";
        
        // Act
        var action = () => _passwordPolicy.Validate(password);

        // Assert
        var exception = Assert.Throws<DomainException>(action);
        
        Assert.Equal("Password needs any upper case letter.", exception.Message);
    }

    //[Fact]
    public void Validate_WithoutSpecialCharacter_ShouldThrowDomainException()
    {
        // Arrange
        const string password = "P1assword";
        
        // Act
        var action = () => _passwordPolicy.Validate(password);

        // Assert
        var exception = Assert.Throws<DomainException>(action);
        
        Assert.Equal("Password needs any special character.", exception.Message);
    }

    //[Fact]
    public void Validate_WithoutDigit_ShouldThrowDomainException()
    {
        // Arrange
        const string password = "P%assword";
        
        // Act
        var action = () => _passwordPolicy.Validate(password);

        var exception = Assert.Throws<DomainException>(action);
        
        Assert.Equal("Password needs any digit.", exception.Message);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithMissingPassword_ShouldThrowDomainException(
        string? password)
    {
        // Act
        var action = () => _passwordPolicy.Validate(password);

        // Assert
        var exception = Assert.Throws<DomainException>(action);
        
        Assert.Equal("Password is required", exception.Message);
    }
}