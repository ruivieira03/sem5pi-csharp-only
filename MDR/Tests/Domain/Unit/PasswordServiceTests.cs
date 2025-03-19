using Xunit;
using Hospital.Services;

namespace Hospital.Tests.Services
{
    public class PasswordServiceTests
    {
        private readonly PasswordService _passwordService;

        public PasswordServiceTests()
        {
            _passwordService = new PasswordService();
        }

        [Fact]
        public void HashPassword_ShouldReturnHash_WhenGivenPassword()
        {
            // Arrange
            var password = "MySecurePassword";

            // Act
            var hashedPassword = _passwordService.HashPassword(password);

            // Assert
            Assert.NotNull(hashedPassword);
            Assert.NotEqual(password, hashedPassword); // Hashed password should not be the same as the original password
            Assert.Equal(64, hashedPassword.Length); // SHA-256 produces a 64-character hash string (256 bits)
        }

        [Fact]
        public void ValidatePassword_ShouldReturnTrue_WhenPasswordMatchesHash()
        {
            // Arrange
            var password = "MySecurePassword";
            var hashedPassword = _passwordService.HashPassword(password);

            // Act
            var isValid = _passwordService.ValidatePassword(password, hashedPassword);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ValidatePassword_ShouldReturnFalse_WhenPasswordDoesNotMatchHash()
        {
            // Arrange
            var password = "MySecurePassword";
            var wrongPassword = "WrongPassword";
            var hashedPassword = _passwordService.HashPassword(password);

            // Act
            var isValid = _passwordService.ValidatePassword(wrongPassword, hashedPassword);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void GenerateTemporaryPassword_ShouldReturnDifferentPasswordEachTime()
        {
            // Act
            string tempPassword1 = _passwordService.GenerateTemporaryPassword("username");
            string tempPassword2 = _passwordService.GenerateTemporaryPassword("username2");

            // Assert
            Assert.NotEqual(tempPassword1, tempPassword2); // Ensure that temporary passwords are unique
        }
    }
}
