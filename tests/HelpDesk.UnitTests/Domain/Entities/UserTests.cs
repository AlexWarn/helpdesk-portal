using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.ValueObjects;

namespace HelpDesk.UnitTests.Domain.Entities;

public sealed class UserTests
{
   [Fact]
   public void Create_WithValidData_ShouldCreateClientUser()
   {
       //Arrange
       const string passwordHash = "hash";
       
       var createdAt = new DateTime(
           2026, 8, 20, 10, 0, 0, 
           DateTimeKind.Utc);
       
       var email = Email.Create("test@email.ru");

       var fullName = FullName.Create(
           "Иван",
           "Петров", 
           "Александрович");
       
       //Act
       var user = User.Create(
           email, 
           passwordHash, 
           fullName, 
           createdAt);
       
       //Assert
       Assert.NotNull(user);
       Assert.Equal(email, user.Email);
       Assert.Equal(fullName, user.FullName);
       Assert.Equal(passwordHash, user.PasswordHash);
       Assert.Equal(createdAt, user.CreatedAtUtc);
       
       Assert.False(user.IsBlocked);
       Assert.Equal(UserRole.Client, user.Role);
       Assert.NotEqual(Guid.Empty, user.Id);
   }

   [Theory]
   [InlineData("")]
   [InlineData("  ")]
   [InlineData(null)]
   public void Create_WithInvalidPasswordHash_ShouldThrowDomainException(
       string? passwordHash)
   {
       // Arrange
       var createdAt = new DateTime(
           2026, 8, 20, 10, 0, 0, 
           DateTimeKind.Utc);
       
       var email = Email.Create("test@email.ru");

       var fullName = FullName.Create(
           "Иван",
           "Петров", 
           "Александрович");
       
       // Act
       var action = () => User.Create(
           email,
           passwordHash,
           fullName,
           createdAt);
       
       // Assert
       Assert.Throws<DomainException>(action);
   }
   
   [Fact]
   public void Create_WithInvalidCreatedAt_ShouldThrowDomainException()
   {
       // Arrange
       DateTime createdAt = default;
       const string passwordHash = "hash";
       var email = Email.Create("test@email.ru");

       var fullName = FullName.Create(
           "Иван",
           "Петров", 
           "Александрович");
       
       // Act
       var action = () => User.Create(
           email,
           passwordHash,
           fullName,
           createdAt);
       
       // Assert
       Assert.Throws<DomainException>(action);
   }
   
   [Fact]
   public void Create_WithInvalidEmail_ShouldThrowDomainException()
   {
       // Arrange
       var createdAt = new DateTime(
           2026, 8, 20, 10, 0, 0, 
           DateTimeKind.Utc);
       
       var passwordHash = "hash";
       
       var email = default(Email);

       var fullName = FullName.Create(
           "Иван",
           "Петров", 
           "Александрович");
       
       // Act
       var action = () => User.Create(
           email,
           passwordHash,
           fullName,
           createdAt);
       
       // Assert
       Assert.Throws<DomainException>(action);
   }
   
   [Fact]
   public void Create_WithInvalidFullName_ShouldThrowDomainException()
   {
       // Arrange
       var createdAt = new DateTime(
           2026, 8, 20, 10, 0, 0, 
           DateTimeKind.Utc);
       
       var email = Email.Create("test@email.ru");
       
       var passwordHash = "hash";

       FullName? fullName = null;
       
       // Act
       var action = () => User.Create(
           email,
           passwordHash,
           fullName,
           createdAt);
       
       // Assert
       Assert.Throws<DomainException>(action);
   }
   
   [Fact]
   public void Block_WhenUserIsNotBlocked_ShouldBlockUser()
   {
      // Arrange
      var user = CreateUser();
      
      // Act
      user.Block();
      
       // Assert
      Assert.True(user.IsBlocked);
   }
   
   [Fact]
   public void Block_WhenUserIsAlreadyBlocked_ShouldThrowDomainException()
   {
       // Arrange
       var user = CreateUser();
      
       // Act
       user.Block();
       
       var action = user.Block;
      
       // Assert
       Assert.Throws<DomainException>(action);
       Assert.True(user.IsBlocked);
   }

   [Fact]
   public void Unblock_WhenUserIsBlocked_ShouldUnblockUser()
   {
       // Arrange
       var user = CreateUser();
       
       user.Block();
       
       // Act
       user.Unblock();
       
       // Assert
       Assert.False(user.IsBlocked);
   }

   [Fact]
   public void Unblock_WhenUserIsNotBlocked_ShouldThrowDomainException()
   {
       // Arrange
       var user = CreateUser();
       
       // Act
       var action = user.Unblock;
       
       // Assert
       Assert.Throws<DomainException>(action);
       Assert.False(user.IsBlocked);
   } 
   
   [Fact]
   public void ChangeEmail_WithDifferentEmail_ShouldChangeEmail()
   {
       // Arrange
       var user = CreateUser();
       var targetEmail = Email.Create("test2@email.ru");
       
       // Act
       user.ChangeEmail(targetEmail);
       
       //Assert
       Assert.Equal(targetEmail, user.Email);
   }

   [Fact]
   public void ChangeEmail_WithSameEmail_ShouldThrowDomainException()
   {
       // Arrange
       var user = CreateUser();
       var originalEmail = user.Email;
       var targetEmail = Email.Create(" TEST@EMAIL.RU ");
       
       // Act
       var action = () => user.ChangeEmail(targetEmail);
       
       // Assert
       Assert.Throws<DomainException>(action);
       Assert.Equal(originalEmail, user.Email);
   }
   
   [Fact]
   public void ChangeEmail_WithEmailNull_ShouldThrowDomainException()
   {
       // Arrange
       var user = CreateUser();
       var originalEmail = user.Email;
       Email? targetEmail = null;
       
       // Act
       var action = () => user.ChangeEmail(targetEmail);
       
       // Assert
       Assert.Throws<DomainException>(action);
       Assert.Equal(originalEmail, user.Email);
   }

   [Fact]
   public void ChangeFullName_WithDifferentFullName_ShouldChangeFullName()
   {
       // Arrange
       var user = CreateUser();
       var targetFullName = FullName.Create("Иван", "Иванов", "Иванович");
       
       // Act
       user.ChangeFullName(targetFullName);
       
       // Assert
       Assert.Equal(targetFullName, user.FullName);
   }

   [Fact]
   public void ChangeFullName_WithSameFullName_ShouldThrowDomainException()
   {
       // Arrange
       var user = CreateUser();
       var originalFullName = user.FullName;
       var targetFullName = FullName.Create(
           " Иван ",
           " Петров",
           "Александрович  ");
       
       // Act
       var action = () => user.ChangeFullName(targetFullName);
       
       // Assert
       Assert.Throws<DomainException>(action);
       Assert.Equal(originalFullName, user.FullName);
   }

   [Fact]
   public void ChangeFullName_WithNullFullName_ShouldThrowDomainException()
   {
       // Arrange
       var user = CreateUser();
       var originalFullName = user.FullName;
       
       // Act
       var action = () => user.ChangeFullName(newFullName: null);
       
       // Assert
       Assert.Throws<DomainException>(action);
       Assert.Equal(originalFullName, user.FullName);
   }

   [Fact]
   public void ChangePasswordHash_WithDifferentValue_ShouldChangePasswordHash()
   {
       // Arrange
       var user = CreateUser();
       const string targetPasswordHash = "differenthash";
       
       // Act
       user.ChangePasswordHash(targetPasswordHash);
       
       // Assert
       Assert.Equal(targetPasswordHash, user.PasswordHash);
   }

   [Fact]
   public void ChangePasswordHash_WithSameValue_ShouldThrowDomainException()
   {
       // Arrange
       var user = CreateUser();
       var originalPasswordHash =user.PasswordHash;
       const string sameHash = "hash";
       
       // Act
       var action = () => user.ChangePasswordHash(sameHash);
       
       // Assert
       Assert.Throws<DomainException>(action);
       Assert.Equal(originalPasswordHash, user.PasswordHash);
   }

   [Theory]
   [InlineData("")]
   [InlineData(" ")]
   [InlineData("  ")]
   [InlineData(null)]
   public void ChangePasswordHash_WithInvalidValue_ShouldThrowDomainException(string? newPasswordHash)
   {
       // Arrange
       var user = CreateUser();
       var originalPasswordHash= user.PasswordHash;
       
       // Act
       var action = () => user.ChangePasswordHash(newPasswordHash);
       
       // Assert
       Assert.Throws<DomainException>(action);
       Assert.Equal(originalPasswordHash, user.PasswordHash);
   }

   [Fact]
   public void AssignRole_WithUndefinedRole_ShouldThrowDomainException()
   {
       // Arrange
       var user = CreateUser();
       var originalRole = user.Role;
       var notExistsRole = (UserRole)999;
       
       // Act
       var action = () => user.AssignRole(notExistsRole);
       
       // Assert
       Assert.Throws<DomainException>(action);
       Assert.Equal(originalRole, user.Role);
   }
   
   [Fact]
   public void AssignRole_WithSameRole_ShouldThrowDomainException()
   {
       // Arrange
       var user = CreateUser();
       var originalRole = user.Role;
       
       // Act
       var action = () => user.AssignRole(originalRole);
       
       // Assert
       Assert.Throws<DomainException>(action);
       Assert.Equal(originalRole, user.Role);
   }
   
   [Fact]
   public void AssignRole_WithDifferentRole_ShouldAssignRole()
   {
       // Arrange
       var user = CreateUser();
       var targetRole = UserRole.Operator;
       
       // Act
       user.AssignRole(targetRole);
       
       // Assert
       Assert.Equal(targetRole, user.Role);
   }
   
   
   private static User CreateUser()
   {
       const string passwordHash = "hash";

       var createdAt = new DateTime(
           2026, 8, 20, 10, 0, 0,
           DateTimeKind.Utc);

       var email = Email.Create("test@email.ru");

       var fullName = FullName.Create(
           "Иван",
           "Петров",
           "Александрович");

       return User.Create(
           email,
           passwordHash,
           fullName,
           createdAt);
   }
}
