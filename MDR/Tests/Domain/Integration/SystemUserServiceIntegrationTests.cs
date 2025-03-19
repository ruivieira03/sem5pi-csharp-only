using Hospital.Domain.Users.SystemUser;
using Hospital.Domain.Patients;
using Hospital.Domain.Shared;
using Hospital.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Hospital.ViewModels;

public class SystemUserServiceIntegrationTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ISystemUserRepository> _mockSystemUserRepository;
    private readonly Mock<IPasswordService> _mockPasswordService;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<IPatientRepository> _mockPatientRepository;
    private readonly Mock<ILoggingService> _mockLoggingService;
    private readonly SystemUserService _systemUserService;

    public SystemUserServiceIntegrationTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockSystemUserRepository = new Mock<ISystemUserRepository>();
        _mockPasswordService = new Mock<IPasswordService>();
        _mockEmailService = new Mock<IEmailService>();
        _mockPatientRepository = new Mock<IPatientRepository>();
        _mockLoggingService = new Mock<ILoggingService>();

        _systemUserService = new SystemUserService(
            _mockUnitOfWork.Object,
            _mockSystemUserRepository.Object,
            _mockPasswordService.Object,
            _mockEmailService.Object,
            _mockPatientRepository.Object,
            _mockLoggingService.Object
        );
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldRegisterUser_WhenValidDataIsProvided()
    {
        // Arrange
        var model = new RegisterUserViewModel
        {
            Username = "newuser",
            Role = Roles.Admin,
            Email = "newuser@example.com",
            PhoneNumber = "1234567890"
        };

        _mockSystemUserRepository
            .Setup(repo => repo.GetUserByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync((SystemUser)null); // Username is not taken

        _mockPasswordService
            .Setup(service => service.GenerateTemporaryPassword(model.Username))
            .Returns("newuser1234");

        _mockPasswordService
            .Setup(service => service.HashPassword(It.IsAny<string>()))
            .Returns("hashedPassword");

        _mockEmailService
            .Setup(service => service.GenerateSetupLink(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("setupLink");

        _mockEmailService
            .Setup(service => service.SendRegistrationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _mockSystemUserRepository
            .Setup(repo => repo.AddUserAsync(It.IsAny<SystemUser>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(uow => uow.CommitAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _systemUserService.RegisterUserAsync(model);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(model.Username, result.Username);
        Assert.Equal(model.Email, result.Email);
        Assert.Equal(Roles.Admin, result.Role);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldThrowException_WhenUsernameIsTaken()
    {
        // Arrange
        var model = new RegisterUserViewModel
        {
            Username = "takenUsername",
            Role = Roles.Admin,
            Email = "newuser@example.com",
            PhoneNumber = "1234567890"
        };

        _mockSystemUserRepository
            .Setup(repo => repo.GetUserByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(new SystemUser()); // Username is taken

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _systemUserService.RegisterUserAsync(model));
        Assert.Equal("Username already taken.", exception.Message);
    }

    [Fact]
    public async Task RegisterPatientUserAsync_ShouldRegisterPatient_WhenValidDataIsProvided()
    {
        // Arrange
        var model = new PatientUserViewModel
        {
            Username = "newpatient",
            Email = "patient@example.com",
            PhoneNumber = "1234567890",
            Password = "password123"
        };

        var patient = new Patient { Email = model.Email };
        _mockSystemUserRepository
            .Setup(repo => repo.GetUserByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync((SystemUser)null); // Username is not taken

        _mockSystemUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((SystemUser)null); // Email is not taken

        _mockPatientRepository
            .Setup(repo => repo.GetPatientByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(patient); // Patient profile exists

        _mockPasswordService
            .Setup(service => service.HashPassword(It.IsAny<string>()))
            .Returns("hashedPassword");

        _mockEmailService
            .Setup(service => service.GenerateEmailVerification(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("verificationLink");

        _mockEmailService
            .Setup(service => service.SendRegistrationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _mockSystemUserRepository
            .Setup(repo => repo.AddUserAsync(It.IsAny<SystemUser>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(uow => uow.CommitAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _systemUserService.RegisterPatientUserAsync(model);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(model.Username, result.Username);
        Assert.Equal(model.Email, result.Email);
        Assert.Equal(Roles.Patient, result.Role);
    }

    [Fact]
    public async Task RegisterPatientUserAsync_ShouldThrowException_WhenPatientProfileDoesNotExist()
    {
        // Arrange
        var model = new PatientUserViewModel
        {
            Username = "newpatient",
            Email = "nonexistentpatient@example.com",
            PhoneNumber = "1234567890",
            Password = "password123"
        };

        _mockSystemUserRepository
            .Setup(repo => repo.GetUserByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync((SystemUser)null); // Username is not taken

        _mockSystemUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((SystemUser)null); // Email is not taken

        _mockPatientRepository
            .Setup(repo => repo.GetPatientByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Patient)null); // Patient profile does not exist

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _systemUserService.RegisterPatientUserAsync(model));
        Assert.Equal("Patient profile for that email doesn't exist.", exception.Message);
    }

    [Fact]
    public async Task RequestPasswordResetAsync_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var email = "nonexistentuser@example.com";

        _mockSystemUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((SystemUser)null); // User not found

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _systemUserService.RequestPasswordResetAsync(email));
        Assert.Equal("User not found.", exception.Message);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldDeleteUser_WhenUserExists()
    {
        // Arrange
        var email = "existinguser@example.com";
        var user = new SystemUser
        {
            Email = email
        };

        _mockSystemUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        _mockSystemUserRepository
            .Setup(repo => repo.Remove(It.IsAny<SystemUser>()))
            .Returns(Task.CompletedTask);

        _mockLoggingService
            .Setup(service => service.LogAccountDeletionAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(uow => uow.CommitAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _systemUserService.DeleteAsync(email);

        // Assert
        Assert.NotNull(result);
        _mockSystemUserRepository.Verify(repo => repo.Remove(It.IsAny<SystemUser>()), Times.Once);
        _mockLoggingService.Verify(service => service.LogAccountDeletionAsync(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var email = "nonexistentuser@example.com";

        _mockSystemUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
            
            .ReturnsAsync((SystemUser)null); // User not found

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _systemUserService.DeleteAsync(email));
        Assert.Equal("User not found.", exception.Message);
    }
}
