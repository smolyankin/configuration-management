using ConfigurationManagement.Application.Auth.Commands.Login;
using ConfigurationManagement.Application.Auth.Commands.RegisterUser;
using ConfigurationManagement.Application.Common.Validation;
using ConfigurationManagement.Application.Configurations.Commands.CreateConfiguration;
using ConfigurationManagement.Application.Configurations.Commands.RestoreConfigurationVersion;
using ConfigurationManagement.Application.Configurations.Commands.UpdateConfiguration;
using ConfigurationManagement.Application.NotificationSubscriptions.Commands.Subscribe;
using ConfigurationManagement.Application.NotificationSubscriptions.Commands.Unsubscribe;
using ConfigurationManagement.Domain.Enums;
using FluentAssertions;

namespace ConfigurationManagement.Tests.Validation;

/// <summary>
/// Unit tests for command validators
/// </summary>
public class CommandValidatorsTests
{
    public class CreateConfigurationCommandValidatorTests : CommandValidatorsTests
    {
        private readonly CreateConfigurationCommandValidator _validator = new();

        [Fact]
        public void Should_Validate_Valid_CreateConfigurationCommand()
        {
            // Arrange
            var command = new CreateConfigurationCommand
            {
                Name = "Test Configuration",
                Data = "{\"key\":\"value\"}"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData("Invalid Name!@#")]
        public void Should_Not_Validate_Invalid_Name(string name)
        {
            // Arrange
            var command = new CreateConfigurationCommand
            {
                Name = name,
                Data = "{\"key\":\"value\"}"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
        }

        [Fact]
        public void Should_Not_Validate_Null_Data()
        {
            // Arrange
            var command = new CreateConfigurationCommand
            {
                Name = "Test Configuration",
                Data = null!
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Data");
        }

        [Fact]
        public void Should_Not_Validate_Name_Exceeding_Maximum_Length()
        {
            // Arrange
            var longName = new string('a', 256);
            var command = new CreateConfigurationCommand
            {
                Name = longName,
                Data = "{\"key\":\"value\"}"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
        }
    }

    public class LoginCommandValidatorTests : CommandValidatorsTests
    {
        private readonly LoginCommandValidator _validator = new();

        [Fact]
        public void Should_Validate_Valid_LoginCommand()
        {
            // Arrange
            var command = new LoginCommand
            {
                Email = "test@example.com",
                Password = "password123"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData("invalid-email")]
        [InlineData("not-an-email")]
        public void Should_Not_Validate_Invalid_Email(string email)
        {
            // Arrange
            var command = new LoginCommand
            {
                Email = email,
                Password = "password123"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }

        [Theory]
        [InlineData("")]
        public void Should_Not_Validate_Invalid_Password(string password)
        {
            // Arrange
            var command = new LoginCommand
            {
                Email = "test@example.com",
                Password = password
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }
    }

    public class RegisterUserCommandValidatorTests : CommandValidatorsTests
    {
        private readonly RegisterUserCommandValidator _validator = new();

        [Fact]
        public void Should_Validate_Valid_RegisterUserCommand()
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Password = "Password123!"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData("invalid-email")]
        public void Should_Not_Validate_Invalid_Email(string email)
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                Email = email,
                FirstName = "John",
                LastName = "Doe",
                Password = "Password123!"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }

        [Theory]
        [InlineData("")]
        [InlineData("John123")]
        public void Should_Not_Validate_Invalid_FirstName(string firstName)
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                Email = "test@example.com",
                FirstName = firstName,
                LastName = "Doe",
                Password = "Password123!"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
        }

        [Theory]
        [InlineData("")]
        [InlineData("Doe123")]
        public void Should_Not_Validate_Invalid_LastName(string lastName)
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = lastName,
                Password = "Password123!"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "LastName");
        }

        [Theory]
        [InlineData("")]
        [InlineData("weak")]
        [InlineData("weakpass")]
        [InlineData("Weakpass")]
        [InlineData("Weakpass1")]
        public void Should_Not_Validate_Invalid_Password(string password)
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Password = password
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }
    }

    public class SubscribeCommandValidatorTests : CommandValidatorsTests
    {
        private readonly SubscribeCommandValidator _validator = new();

        [Fact]
        public void Should_Validate_Empty_ConfigurationEventTypes()
        {
            // Arrange
            var command = new SubscribeCommand
            {
                ConfigurationEventTypes = new List<ConfigurationEventType>()
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Should_Validate_Valid_ConfigurationEventTypes()
        {
            // Arrange
            var command = new SubscribeCommand
            {
                ConfigurationEventTypes = new List<ConfigurationEventType>
                {
                    ConfigurationEventType.Created,
                    ConfigurationEventType.Modified
                }
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Should_Validate_All_ConfigurationEventTypes()
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

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
    }

    public class UnsubscribeCommandValidatorTests : CommandValidatorsTests
    {
        private readonly UnsubscribeCommandValidator _validator = new();

        [Fact]
        public void Should_Always_Validate_UnsubscribeCommand()
        {
            // Arrange
            var command = new UnsubscribeCommand();

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
    }

    public class UpdateConfigurationCommandValidatorTests : CommandValidatorsTests
    {
        private readonly UpdateConfigurationCommandValidator _validator = new();

        [Fact]
        public void Should_Validate_Valid_UpdateConfigurationCommand()
        {
            // Arrange
            var command = new UpdateConfigurationCommand
            {
                Id = Guid.NewGuid(),
                Name = "Updated Configuration",
                Data = "{\"key\":\"updated_value\"}"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Should_Not_Validate_Empty_Id()
        {
            // Arrange
            var command = new UpdateConfigurationCommand
            {
                Id = Guid.Empty,
                Name = "Updated Configuration",
                Data = "{\"key\":\"updated_value\"}"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Id");
        }

        [Theory]
        [InlineData("")]
        [InlineData("Invalid Name!@#")]
        public void Should_Not_Validate_Invalid_Name(string name)
        {
            // Arrange
            var command = new UpdateConfigurationCommand
            {
                Id = Guid.NewGuid(),
                Name = name,
                Data = "{\"key\":\"updated_value\"}"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
        }

        [Fact]
        public void Should_Not_Validate_Name_Exceeding_Maximum_Length()
        {
            // Arrange
            var longName = new string('a', 256);
            var command = new UpdateConfigurationCommand
            {
                Id = Guid.NewGuid(),
                Name = longName,
                Data = "{\"key\":\"updated_value\"}"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
        }
    }

    public class RestoreConfigurationVersionCommandValidatorTests : CommandValidatorsTests
    {
        private readonly RestoreConfigurationVersionCommandValidator _validator = new();

        [Fact]
        public void Should_Validate_Valid_RestoreConfigurationVersionCommand()
        {
            // Arrange
            var command = new RestoreConfigurationVersionCommand
            {
                ConfigurationId = Guid.NewGuid(),
                VersionNumber = 2
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Should_Not_Validate_Empty_ConfigurationId()
        {
            // Arrange
            var command = new RestoreConfigurationVersionCommand
            {
                ConfigurationId = Guid.Empty,
                VersionNumber = 1
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ConfigurationId");
        }

        [Fact]
        public void Should_Not_Validate_Zero_VersionNumber()
        {
            // Arrange
            var command = new RestoreConfigurationVersionCommand
            {
                ConfigurationId = Guid.NewGuid(),
                VersionNumber = 0
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "VersionNumber");
        }
    }
}