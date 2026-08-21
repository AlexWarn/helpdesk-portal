using HelpDesk.Domain.ValueObjects;

namespace HelpDesk.UnitTests.Domain.ValueObjects;

public sealed class EmailTests
{
    [Fact]
    public void Create_WithValidValue_ShouldCreateEmail()
    {
        // Arrange
        const string value = "user@example.com";

        // Act
        var email = Email.Create(value);

        // Assert
        Assert.Equal(value, email.Value);
    }
    
    [Fact]
    public void Create_WithSpacesAndUppercase_ShouldNormalizeValue()
    {
        // Arrange
        const string value = "  User@Example.COM  ";

        // Act
        var email = Email.Create(value);

        // Assert
        Assert.Equal("user@example.com", email.Value);
    }

    [Fact]
    public void ToString_ShouldReturnNormalizedValue()
    {
        // Arrange
        const string value = "  User@Example.COM  ";

        // Act
        var email = Email.Create(value);

        // Assert
        Assert.Equal("user@example.com", email.ToString());
    }

    [Fact]
    public void TwoEmails_WithDifferentCase_ShouldBeEqual()
    {
        const string value1 = "ASd@Rty.ru";
        const string value2 = "aSd@rTy.ru";
        
        var email1 = Email.Create(value1);
        var email2 = Email.Create(value2);
        
        Assert.Equal(email1, email2);
    }
    
}