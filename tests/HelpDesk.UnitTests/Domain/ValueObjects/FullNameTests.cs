using HelpDesk.Domain.ValueObjects;

namespace HelpDesk.UnitTests.Domain.ValueObjects;

public class FullNameTests
{
    [Fact]
    public void Create_WhenMiddleNameIsEmpty_MiddleNameSetNull()
    {
        const string firstName = "Alex";
        const string lastName = "Warn";
        var middleName = string.Empty;

        
        var fullName = FullName.Create(
            firstName,
            lastName,
            middleName);
        
        
        Assert.Equal($"{lastName} {firstName}", fullName.Value);
    }
}