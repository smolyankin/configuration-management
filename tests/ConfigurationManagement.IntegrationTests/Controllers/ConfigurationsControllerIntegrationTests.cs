using ConfigurationManagement.Api.Controllers;
using ConfigurationManagement.Application.Configurations.Commands.CreateConfiguration;
using ConfigurationManagement.Application.Configurations.Commands.RestoreConfigurationVersion;
using ConfigurationManagement.Application.Configurations.Commands.UpdateConfiguration;
using ConfigurationManagement.Application.Configurations.Dto;
using ConfigurationManagement.Application.Configurations.Queries.GetConfigurationById;
using ConfigurationManagement.Application.Configurations.Queries.GetConfigurations;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ConfigurationManagement.IntegrationTests.Controllers;

public class ConfigurationsControllerIntegrationTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILogger<ConfigurationsController>> _mockLogger;
    private readonly ConfigurationsController _controller;

    public ConfigurationsControllerIntegrationTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockLogger = new Mock<ILogger<ConfigurationsController>>();
        _controller = new ConfigurationsController(_mockMediator.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateConfiguration_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var command = new CreateConfigurationCommand
        {
            Name = "Test Configuration",
            Data = "{\"key\": \"value\", \"setting\": true}"
        };

        var createdConfiguration = new ConfigurationDto
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Data = command.Data,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(command, default))
                     .ReturnsAsync(createdConfiguration);

        // Act
        var result = await _controller.CreateConfiguration(command, CancellationToken.None);

        // Assert
        var createdAtResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtResult.ActionName.Should().Be(nameof(ConfigurationsController.GetConfiguration));
        createdAtResult.RouteValues!["id"].Should().Be(createdConfiguration.Id);
        createdAtResult.Value.Should().BeEquivalentTo(createdConfiguration);

        _mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateConfiguration_WithEmptyName_ShouldCallMediator()
    {
        // Arrange
        var command = new CreateConfigurationCommand
        {
            Name = "",
            Data = "{\"key\": \"value\"}"
        };

        var createdConfiguration = new ConfigurationDto
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Data = command.Data,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(command, default))
                     .ReturnsAsync(createdConfiguration);

        // Act
        var result = await _controller.CreateConfiguration(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        _mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetConfigurations_ShouldReturnOk()
    {
        // Arrange
        var query = new GetConfigurationsQuery
        {
            Name = "Test"
        };

        var configurations = new List<ConfigurationDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Test Config 1",
                Data = "{\"test\": true}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Test Config 2",
                Data = "{\"test\": false}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var response = new ConfigurationsDto
        {
            Configurations = configurations,
            Pagination = new PaginationInfo { PageNumber = 1, PageSize = 10, TotalItems = 2 }
        };

        _mockMediator.Setup(x => x.Send(query, default))
                     .ReturnsAsync(response);

        // Act
        var result = await _controller.GetConfigurations(query, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeOfType<ConfigurationsDto>().Subject;
        value.Configurations.Should().HaveCount(2);
        value.Configurations.Should().Contain(c => c.Name.Contains("Test"));

        _mockMediator.Verify(x => x.Send(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetConfiguration_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var configId = Guid.NewGuid();
        var query = new GetConfigurationByIdQuery { Id = configId };

        var configuration = new ConfigurationDto
        {
            Id = configId,
            Name = "Test Configuration",
            Data = "{\"key\": \"value\"}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(query, default))
                     .ReturnsAsync(configuration);

        // Act
        var result = await _controller.GetConfiguration(configId, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeOfType<ConfigurationDto>().Subject;
        value.Id.Should().Be(configId);
        value.Name.Should().Be("Test Configuration");

        _mockMediator.Verify(x => x.Send(It.Is<GetConfigurationByIdQuery>(q => q.Id == configId),
                                          It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetConfiguration_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var configId = Guid.NewGuid();
        var query = new GetConfigurationByIdQuery { Id = configId };

        _mockMediator.Setup(x => x.Send(query, default))
                     .ReturnsAsync((ConfigurationDto?)null);

        // Act
        var result = await _controller.GetConfiguration(configId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        _mockMediator.Verify(x => x.Send(It.Is<GetConfigurationByIdQuery>(q => q.Id == configId),
                                          It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateConfiguration_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var configId = Guid.NewGuid();
        var command = new UpdateConfigurationCommand
        {
            Id = configId,
            Name = "Updated Configuration",
            Data = "{\"updated\": \"data\"}"
        };

        var updatedConfiguration = new ConfigurationDto
        {
            Id = configId,
            Name = command.Name,
            Data = command.Data,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(command, default))
                     .ReturnsAsync(updatedConfiguration);

        // Act
        var result = await _controller.UpdateConfiguration(configId, command, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeOfType<ConfigurationDto>().Subject;
        value.Id.Should().Be(configId);
        value.Name.Should().Be("Updated Configuration");
        value.Data.Should().Be("{\"updated\": \"data\"}");

        command.Id.Should().Be(configId);

        _mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RestoreConfigurationVersion_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var configId = Guid.NewGuid();
        var command = new RestoreConfigurationVersionCommand
        {
            ConfigurationId = Guid.Empty,
            VersionNumber = 2
        };

        var restoredConfiguration = new ConfigurationDto
        {
            Id = configId,
            Name = "Restored Configuration",
            Data = "{\"restored\": \"data\"}",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(It.Is<RestoreConfigurationVersionCommand>(c => c.ConfigurationId == configId && c.VersionNumber == 2), default))
                     .ReturnsAsync(restoredConfiguration);

        // Act
        var result = await _controller.RestoreConfigurationVersion(configId, command, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeOfType<ConfigurationDto>().Subject;
        value.Id.Should().Be(configId);
        value.Name.Should().Be("Restored Configuration");

        _mockMediator.Verify(x => x.Send(It.Is<RestoreConfigurationVersionCommand>(c => c.ConfigurationId == configId && c.VersionNumber == 2),
                                          It.IsAny<CancellationToken>()), Times.Once);
    }
}