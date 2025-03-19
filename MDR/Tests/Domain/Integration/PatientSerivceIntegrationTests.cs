using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Hospital.Domain.Patients;
using Hospital.Domain.Shared;
using Hospital.Services;
using Hospital.ViewModels;
using Hospital.Domain.Users.SystemUser;

public class PatientServiceIntegrationTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IPatientRepository> _mockPatientRepository;
    private readonly Mock<ISystemUserRepository> _mockSystemUserRepository;
    private readonly Mock<ILoggingService> _mockLoggingService;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly PatientService _patientService;

    public PatientServiceIntegrationTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockPatientRepository = new Mock<IPatientRepository>();
        _mockSystemUserRepository = new Mock<ISystemUserRepository>();
        _mockLoggingService = new Mock<ILoggingService>();
        _mockEmailService = new Mock<IEmailService>();

        _patientService = new PatientService(
            _mockPatientRepository.Object,
            _mockUnitOfWork.Object,
            _mockSystemUserRepository.Object,
            _mockLoggingService.Object,
            _mockEmailService.Object
        );
    }

    [Fact]
    public async Task UpdateProfileAsUserAsync_ShouldUpdatePatientProfile_WhenValidDataIsProvided()
    {
        // Arrange
        var userId = new SystemUserId(Guid.NewGuid());
        var existingUser = new SystemUser
        {
            Username = "existingUser",
            Email = "user@example.com",
            PhoneNumber = "123456789",
            IAMId = "IAM123"
        };

        var existingPatient = new Patient
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "user@example.com",
            PhoneNumber = "123456789",
            EmergencyContact = "Jane Doe"
        };

        var model = new UpdateProfileViewModel
        {
            FirstName = "UpdatedFirstName",
            LastName = "UpdatedLastName",
            Email = "updated@example.com",
            PhoneNumber = "987654321",
            EmergencyContact = "Updated Contact"
        };

        _mockSystemUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(existingUser);
        _mockPatientRepository.Setup(repo => repo.GetPatientByEmailAsync(existingUser.Email)).ReturnsAsync(existingPatient);
        _mockLoggingService.Setup(log => log.LogProfileUpdateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.CompletedTask);
        _mockEmailService.Setup(email => email.SendEmailConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(uow => uow.CommitAsync()).ReturnsAsync(1); // Alterado para ReturnsAsync(1)

        // Act
        var result = await _patientService.UpdateProfileAsUserAsync(model, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("UpdatedFirstName", result.FirstName);
        Assert.Equal("updated@example.com", result.Email);
        _mockLoggingService.Verify(log => log.LogProfileUpdateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
    }

  

    [Fact]
    public async Task UpdateProfileAsUserAsync_ShouldThrowException_WhenPatientNotFound()
    {
        // Arrange
        var userId = new SystemUserId(Guid.NewGuid());
        var existingUser = new SystemUser { Email = "nonexistent@example.com" };

        _mockSystemUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(existingUser);
        _mockPatientRepository.Setup(repo => repo.GetPatientByEmailAsync(existingUser.Email)).ReturnsAsync((Patient)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _patientService.UpdateProfileAsUserAsync(new UpdateProfileViewModel(), userId));
        Assert.Equal("Patient not found.", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeletePatient_WhenPatientExists()
    {
        // Arrange
        var patientId = new PatientId(Guid.NewGuid());
        var existingPatient = new Patient { Id = patientId };

        _mockPatientRepository.Setup(repo => repo.GetByIdAsync(patientId)).ReturnsAsync(existingPatient);
        _mockPatientRepository.Setup(repo => repo.Remove(existingPatient)).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(uow => uow.CommitAsync()).ReturnsAsync(1); // Alterado para ReturnsAsync(1)

        // Act
        await _patientService.DeleteAsync(patientId);

        // Assert
        _mockPatientRepository.Verify(repo => repo.Remove(existingPatient), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowException_WhenPatientNotFound(){
        // Arrange
        var patientId = new PatientId(Guid.NewGuid());
        _mockPatientRepository.Setup(repo => repo.GetByIdAsync(patientId)).ReturnsAsync((Patient)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _patientService.DeleteAsync(patientId));
        Assert.Equal("Patient not found.", exception.Message);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPatients()
    {
        // Arrange
        var patients = new List<Patient>
        {
            new Patient { FirstName = "Patient1", Email = "patient1@example.com" },
            new Patient { FirstName = "Patient2", Email = "patient2@example.com" }
        };

        _mockPatientRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(patients);

        // Act
        var result = await _patientService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Patient1", result[0].FirstName);
        Assert.Equal("Patient2", result[1].FirstName);
    }
}
