using ConfigurationManagement.Api.Controllers;
using ConfigurationManagement.Application.Auth.Commands.Login;
using ConfigurationManagement.Application.Auth.Commands.RegisterUser;
using ConfigurationManagement.Application.Auth.Dto;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ConfigurationManagement.IntegrationTests.Controllers;

public class AuthControllerIntegrationTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly AuthController _controller;

    public AuthControllerIntegrationTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockLogger = new Mock<ILogger<AuthController>>();
        _controller = new AuthController(_mockMediator.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task RegisterUser_WithValidData_ShouldReturnNoContent()
    {
        // Arrange
        var command = new RegisterUserCommand
        {
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            Password = "TestPassword123!"
        };

        // Act
        var result = await _controller.Register(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterUser_WithEmptyPassword_ShouldCallMediator()
    {
        // Arrange
        var command = new RegisterUserCommand
        {
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            Password = ""
        };

        // Act
        var result = await _controller.Register(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOk()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "TestPassword123!"
        };

        var authResponse = new AuthDto
        {
            Token = "jwt-token",
            UserId = Guid.NewGuid(),
            Email = command.Email,
            FullName = "Test User",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        _mockMediator.Setup(x => x.Send(command, default))
                     .ReturnsAsync(authResponse);

        // Act
        var result = await _controller.Login(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Login_WithEmptyPassword_ShouldCallMediator()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = ""
        };

        var authResponse = new AuthDto
        {
            Token = "jwt-token",
            UserId = Guid.NewGuid(),
            Email = command.Email,
            FullName = "Test User",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        _mockMediator.Setup(x => x.Send(command, default))
                     .ReturnsAsync(authResponse);

        // Act
        var result = await _controller.Login(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "nonexistent@example.com",
            Password = "WrongPassword123!"
        };

        _mockMediator.Setup(x => x.Send(command, default))
                     .ThrowsAsync(new UnauthorizedAccessException());

        // Act
        var result = await _controller.Login(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
        _mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }
}