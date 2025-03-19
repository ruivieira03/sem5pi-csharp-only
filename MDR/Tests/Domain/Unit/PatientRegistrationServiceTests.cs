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

namespace Hospital.Tests.Domain.Unit{
    public class PatientRegistrationServiceTests
    {
        private readonly Mock<IPatientRepository> _patientRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ISystemUserRepository> _systemUserRepositoryMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly PatientRegistrationService _patientRegistrationService;

        public PatientRegistrationServiceTests()
        {
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _systemUserRepositoryMock = new Mock<ISystemUserRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _patientRegistrationService = new PatientRegistrationService(_unitOfWorkMock.Object, _systemUserRepositoryMock.Object, _emailServiceMock.Object, _patientRepositoryMock.Object);
        }

        [Fact]
        public async Task RegisterPatientProfileAsync_ShouldThrowException_WhenEmailIsTaken(){  // Test Email Uniqueness
            // Arrange
            var model = new RegisterPatientProfileViewModel { Email = "test@example.com", PhoneNumber = "123456789" };
            _patientRepositoryMock.Setup(r => r.GetPatientByEmailAsync(model.Email)).ReturnsAsync(new Patient());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _patientRegistrationService.RegisterPatientProfileAsync(model));
        }

        [Fact]
        public async Task RegisterPatientProfileAsync_ShouldThrowException_WhenPhoneNumberIsTaken()  // Test PhoneNumber Uniqueness
        {
            // Arrange
            var model = new RegisterPatientProfileViewModel { Email = "unique@example.com", PhoneNumber = "123456789" };
            _patientRepositoryMock.Setup(r => r.GetPatientByPhoneNumberAsync(model.PhoneNumber)).ReturnsAsync(new Patient());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _patientRegistrationService.RegisterPatientProfileAsync(model));
        }


        // Removed Patient Is insterted on Database and Removed.



        [Fact]
        public void GenerateMedicalRecordNumber_ShouldGenerateCorrectFormat(){   // Medical RecordNumber test
            // Arrange
            _patientRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Patient> { new Patient(), new Patient() });

            // Act
            var result = _patientRegistrationService.GenerateMedicalRecordNumber();

            // Assert
            Assert.StartsWith(DateTime.Now.ToString("yyyyMM"), result);             // Test Correct Format here month , yea t, and Number of Patients
            Assert.EndsWith("000002", result);
        }
    }
}
