using ConfigurationManagement.Api.Controllers;
using ConfigurationManagement.Application.NotificationSubscriptions.Commands.Subscribe;
using ConfigurationManagement.Application.NotificationSubscriptions.Commands.Unsubscribe;
using ConfigurationManagement.Application.NotificationSubscriptions.Dto;
using ConfigurationManagement.Domain.Enums;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ConfigurationManagement.IntegrationTests.Controllers;

public class NotificationSubscriptionsControllerIntegrationTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILogger<NotificationSubscriptionsController>> _mockLogger;
    private readonly NotificationSubscriptionsController _controller;

    public NotificationSubscriptionsControllerIntegrationTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockLogger = new Mock<ILogger<NotificationSubscriptionsController>>();
        _controller = new NotificationSubscriptionsController(_mockMediator.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Subscribe_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new SubscribeCommand
        {
            ConfigurationEventTypes = new List<ConfigurationEventType>
            {
                ConfigurationEventType.Created,
                ConfigurationEventType.Modified
            }
        };

        var subscriptionDto = new NotificationSubscriptionDto
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EventTypes = command.ConfigurationEventTypes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(command, default))
                     .ReturnsAsync(subscriptionDto);

        // Act
        var result = await _controller.Subscribe(command, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeOfType<NotificationSubscriptionDto>().Subject;
        value.Id.Should().Be(subscriptionDto.Id);
        value.UserId.Should().Be(userId);
        value.EventTypes.Should().BeEquivalentTo(command.ConfigurationEventTypes);

        _mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Subscribe_WithEmptyEventTypes_ShouldCallMediator()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new SubscribeCommand
        {
            ConfigurationEventTypes = new List<ConfigurationEventType>()
        };

        var subscriptionDto = new NotificationSubscriptionDto
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EventTypes = command.ConfigurationEventTypes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(command, default))
                     .ReturnsAsync(subscriptionDto);

        // Act
        var result = await _controller.Subscribe(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Subscribe_WithAllEventTypes_ShouldReturnOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new SubscribeCommand
        {
            ConfigurationEventTypes = new List<ConfigurationEventType>
            {
                ConfigurationEventType.Created,
                ConfigurationEventType.Modified,
                ConfigurationEventType.Restored
            }
        };

        var subscriptionDto = new NotificationSubscriptionDto
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EventTypes = command.ConfigurationEventTypes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(command, default))
                     .ReturnsAsync(subscriptionDto);

        // Act
        var result = await _controller.Subscribe(command, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeOfType<NotificationSubscriptionDto>().Subject;
        value.EventTypes.Should().HaveCount(3);
        value.EventTypes.Should().Contain(ConfigurationEventType.Created);
        value.EventTypes.Should().Contain(ConfigurationEventType.Modified);
        value.EventTypes.Should().Contain(ConfigurationEventType.Restored);

        _mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Unsubscribe_WithValidData_ShouldReturnNoContent()
    {
        // Arrange
        var command = new UnsubscribeCommand();

        _mockMediator.Setup(x => x.Send(command, default))
                     .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Unsubscribe(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        _mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Subscribe_MultipleSubscriptionTypes_ShouldHandleAllTypes()
    {
        // Arrange
        var command = new SubscribeCommand
        {
            ConfigurationEventTypes = new List<ConfigurationEventType>
            {
                ConfigurationEventType.Created,
                ConfigurationEventType.Modified,
                ConfigurationEventType.Restored
            }
        };

        var subscriptionDto = new NotificationSubscriptionDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            EventTypes = command.ConfigurationEventTypes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(command, default))
                     .ReturnsAsync(subscriptionDto);

        // Act
        var result = await _controller.Subscribe(command, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeOfType<NotificationSubscriptionDto>().Subject;
        value.EventTypes.Should().HaveCount(3);
        value.EventTypes.Should().ContainInOrder(
            ConfigurationEventType.Created,
            ConfigurationEventType.Modified,
            ConfigurationEventType.Restored
        );

        _mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }
}