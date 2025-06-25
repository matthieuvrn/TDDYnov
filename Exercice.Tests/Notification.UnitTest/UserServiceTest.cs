using FluentAssertions;
using Moq;
using Notification.Contracts;
using Notification.Model;
using Notification.Services;

namespace Notification.UnitTest;

public class UserServiceTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly UserService _userService;

    public UserServiceTest()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _userService = new UserService(_userRepositoryMock.Object, _emailServiceMock.Object);
    }

    [Fact]
    public void CreateUser_WhenUserDoesNotExist_ShouldCreateUserAndSendEmail()
    {
        // Arrange
        var name = "John Doe";
        var email = "john.doe@example.com";
        var expectedUser = new User { Name = name, Email = email };

        _userRepositoryMock.Setup(x => x.Exists(email)).Returns(false);
        _userRepositoryMock.Setup(x => x.Save(It.IsAny<User>()));
        _emailServiceMock.Setup(x => x.SendWelcomeEmail(email, name));

        // Act
        var result = _userService.CreateUser(name, email);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(name);
        result.Email.Should().Be(email);

        _userRepositoryMock.Verify(x => x.Exists(email), Times.Once);
        _userRepositoryMock.Verify(x => x.Save(It.Is<User>(u => u.Name == name && u.Email == email)), Times.Once);
        _emailServiceMock.Verify(x => x.SendWelcomeEmail(email, name), Times.Once);
    }

    [Fact]
    public void CreateUser_WhenUserExists_ShouldThrowException()
    {
        // Arrange
        var name = "John Doe";
        var email = "existing@example.com";

        _userRepositoryMock.Setup(x => x.Exists(email)).Returns(true);

        // Act & Assert
        var action = () => _userService.CreateUser(name, email);

        action.Should().Throw<InvalidOperationException>()
              .WithMessage("User already exists");

        _userRepositoryMock.Verify(x => x.Exists(email), Times.Once);
        _userRepositoryMock.Verify(x => x.Save(It.IsAny<User>()), Times.Never);
        _emailServiceMock.Verify(x => x.SendWelcomeEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void GetUser_WithValidId_ShouldReturnUser()
    {
        // Arrange
        var userId = 1;
        var expectedUser = new User { Id = userId, Name = "John Doe", Email = "john@example.com" };

        _userRepositoryMock.Setup(x => x.GetById(userId)).Returns(expectedUser);

        // Act
        var result = _userService.GetUser(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedUser);
        _userRepositoryMock.Verify(x => x.GetById(userId), Times.Once);
    }

    [Fact]
    public void GetUser_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var userId = 999;

        _userRepositoryMock.Setup(x => x.GetById(userId)).Returns((User)null);

        // Act
        var result = _userService.GetUser(userId);

        // Assert
        result.Should().BeNull();
        _userRepositoryMock.Verify(x => x.GetById(userId), Times.Once);
    }

    [Theory]
    [InlineData("", "test@example.com")]
    [InlineData(null, "test@example.com")]
    [InlineData("John", "")]
    [InlineData("John", null)]
    public void CreateUser_WithInvalidParameters_ShouldHandleGracefully(string name, string email)
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);

        // Act
        var result = _userService.CreateUser(name, email);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(name);
        result.Email.Should().Be(email);

        _userRepositoryMock.Verify(x => x.Save(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public void CreateUser_WhenRepositorySaveThrows_ShouldPropagateException()
    {
        // Arrange
        var name = "John Doe";
        var email = "john@example.com";

        _userRepositoryMock.Setup(x => x.Exists(email)).Returns(false);
        _userRepositoryMock.Setup(x => x.Save(It.IsAny<User>()))
                           .Throws(new Exception("Database error"));

        // Act & Assert
        var action = () => _userService.CreateUser(name, email);

        action.Should().Throw<Exception>()
              .WithMessage("Database error");

        _emailServiceMock.Verify(x => x.SendWelcomeEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void CreateUser_WithCallback_CapturesUserData()
    {
        // Arrange
        var name = "John Doe";
        var email = "john@example.com";
        User capturedUser = null;

        _userRepositoryMock.Setup(x => x.Exists(email)).Returns(false);
        _userRepositoryMock.Setup(x => x.Save(It.IsAny<User>()))
                           .Callback<User>(user => capturedUser = user);

        // Act
        var result = _userService.CreateUser(name, email);

        // Assert
        capturedUser.Should().NotBeNull();
        capturedUser.Name.Should().Be(name);
        capturedUser.Email.Should().Be(email);
        capturedUser.Id.Should().Be(0); // default value for new user
    }
}
