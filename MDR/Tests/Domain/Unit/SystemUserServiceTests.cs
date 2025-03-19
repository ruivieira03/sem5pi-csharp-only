using Moq;
using FluentAssertions;
using Hospital.Domain.Shared;
using Hospital.Services;
using Hospital.Domain.Patients;
using Hospital.ViewModels;
using Hospital.Domain.Users.SystemUser;
using Xunit;

namespace Hospital.Tests.Domain.Unit
{
    public class SystemUserServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ISystemUserRepository> _mockSystemUserRepository;
        private readonly Mock<IPasswordService> _mockPasswordService;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IPatientRepository> _mockPatientRepository;
        private readonly Mock<ILoggingService> _mockLoggingService;

        private readonly SystemUserService _systemUserService;

        public SystemUserServiceTests()
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
        public async Task RegisterUserAsync_ShouldThrowException_WhenUsernameIsTaken()
        {
            // Arrange
            var model = new RegisterUserViewModel { Username = "existingUser", Email = "test@example.com" };
            _mockSystemUserRepository.Setup(repo => repo.GetUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync(new SystemUser());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _systemUserService.RegisterUserAsync(model));
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldSendEmail_WhenUserIsRegisteredSuccessfully()
        {
            // Arrange
            var model = new RegisterUserViewModel
            {
                Username = "newUser",
                Email = "newuser@example.com",
                PhoneNumber = "1234567890",
                Role = Enum.Parse<Roles>("Admin", true)
            };
            _mockSystemUserRepository.Setup(repo => repo.GetUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync((SystemUser)null);
            _mockPasswordService.Setup(ps => ps.GenerateTemporaryPassword(model.Username)).Returns("newUser1234");
            _mockPasswordService.Setup(ps => ps.HashPassword(It.IsAny<string>())).Returns("hashedPassword");
            _mockEmailService.Setup(es => es.GenerateSetupLink(It.IsAny<string>(), It.IsAny<string>())).Returns("setupLink");
            _mockEmailService.Setup(es => es.SendRegistrationEmailAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            var result = await _systemUserService.RegisterUserAsync(model);

            // Assert
            result.Should().NotBeNull();
            result.Username.Should().Be("newUser");
            _mockEmailService.Verify(es => es.SendRegistrationEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RegisterPatientUserAsync_ShouldThrowException_WhenUsernameIsTaken()
        {
            // Arrange
            var model = new PatientUserViewModel { Username = "existingUser", Email = "test@example.com" };
            _mockSystemUserRepository.Setup(repo => repo.GetUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync(new SystemUser());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _systemUserService.RegisterPatientUserAsync(model));
        }

        [Fact]
        public async Task RequestPasswordResetAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            string email = "nonexistent@example.com";
            _mockSystemUserRepository.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync((SystemUser)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _systemUserService.RequestPasswordResetAsync(email));
        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            string email = "nonexistent@example.com";
            _mockSystemUserRepository.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync((SystemUser)null);
            string token = "token";

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _systemUserService.ResetPasswordAsync(email, "newPassword", token));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnUsers_WhenUsersExist()
        {
            // Arrange
            var users = new List<SystemUser>
            {
                new SystemUser { Username = "user1", Email = "user1@example.com" },
                new SystemUser { Username = "user2", Email = "user2@example.com" }
            };
            _mockSystemUserRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _systemUserService.GetAllAsync();

            // Assert
            result.Should().NotBeEmpty();
            result.Count.Should().Be(2);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var userId = new SystemUserId(Guid.NewGuid());
            _mockSystemUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((SystemUser)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _systemUserService.GetByIdAsync(userId));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            string email = "nonexistent@example.com";
            _mockSystemUserRepository.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync((SystemUser)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _systemUserService.DeleteAsync(email));
        }

        [Fact]
        public async Task InactivateAsync_ShouldReturnInactivatedUser_WhenUserExists(){
            // Arrange
            var userId = new SystemUserId(Guid.NewGuid());
            var user = new SystemUser { Id = userId, Username = "user1", Email = "user1@example.com" };
            _mockSystemUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _systemUserService.InactivateAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.ResetToken.Should().BeNull();
        }

        [Fact]
        public async Task DeleteFromIdAsync_ShouldThrowException_WhenUserNotFound(){
            // Arrange
            var userId = new SystemUserId(Guid.NewGuid());
            _mockSystemUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((SystemUser)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _systemUserService.DeleteFromIdAsync(userId));
        }

        [Fact]
        public async Task ConfirmEmailAsync_ShouldThrowException_WhenUserNotFound(){
            // Arrange
            string email = "nonexistent@example.com";
            string token = "someToken";
            _mockSystemUserRepository.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync((SystemUser)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _systemUserService.ConfirmEmailAsync(email, token));
        }

        [Fact]
        public async Task ValidateEmailTokenAsync_ShouldReturnFalse_WhenTokenInvalid(){
            // Arrange
            string email = "user@example.com";
            string token = "invalidToken";
            var user = new SystemUser { Email = email, VerifyToken = "validToken", TokenExpiry = DateTime.UtcNow.AddHours(1) };
            _mockSystemUserRepository.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(user);

            // Act
            var result = await _systemUserService.ValidateEmailTokenAsync(email, token);

            // Assert
            result.Should().BeFalse();
        }
    }
}
