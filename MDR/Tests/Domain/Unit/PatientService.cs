using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Hospital.Domain.Patients;
using Hospital.Domain.Shared;
using Hospital.Domain.Users.SystemUser;
using Hospital.Services;
using Hospital.ViewModels;

namespace Hospital.Tests.Domain.Unit
{
    public class PatientServiceTests
    {
        private readonly Mock<IPatientRepository> _patientRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ISystemUserRepository> _systemUserRepositoryMock;
        private readonly Mock<ILoggingService> _loggingServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly PatientService _patientService;

        public PatientServiceTests()
        {
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _systemUserRepositoryMock = new Mock<ISystemUserRepository>();
            _loggingServiceMock = new Mock<ILoggingService>();
            _emailServiceMock = new Mock<IEmailService>();
            _patientService = new PatientService(_patientRepositoryMock.Object, _unitOfWorkMock.Object, _systemUserRepositoryMock.Object, _loggingServiceMock.Object, _emailServiceMock.Object);
        }

       
        [Fact]
        public async Task UpdateProfileAsUserAsync_ShouldThrowException_WhenUserNotFound()   // Throw exception for user 
        {
            // Arrange
            _systemUserRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<SystemUserId>())).ReturnsAsync((SystemUser)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _patientService.UpdateProfileAsUserAsync(new UpdateProfileViewModel(), new SystemUserId(Guid.NewGuid())));
        }

        [Fact]
        public async Task UpdateProfileAsync_ShouldUpdatePatientAndCommit()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var model = new UpdatePatientProfileViewModel { FirstName = "John", LastName = "Doe", Email = "john@example.com", PhoneNumber = "123456789", EmergencyContact = "Jane Doe" };
            var patient = new Patient { Id = new PatientId(patientId), FirstName = "OldName", Email = "old@example.com" };
            
            _patientRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<PatientId>())).ReturnsAsync(patient);

            // Act
            var result = await _patientService.UpdateProfileAsync(model, patientId);

            // Assert
            Assert.Equal(model.FirstName, result.FirstName);
            Assert.Equal(model.LastName, result.LastName);
            Assert.Equal(model.Email, result.Email);
            _patientRepositoryMock.Verify(r => r.UpdatePatientAsync(patient), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemovePatientAndCommit()    // Test User Storie Functionality 
        {
            // Arrange
            var patientId = new PatientId(Guid.NewGuid());
            var patient = new Patient { Id = patientId };
            _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync(patient);

            // Act
            await _patientService.DeleteAsync(patientId);

            // Assert
            _patientRepositoryMock.Verify(r => r.Remove(patient), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllPatients()
        {
            // Arrange
            var patients = new List<Patient> { new Patient { FirstName = "John" }, new Patient { FirstName = "Jane" } };
            _patientRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(patients);

            // Act
            var result = await _patientService.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("John", result[0].FirstName);
            Assert.Equal("Jane", result[1].FirstName);
        }
    }
}
