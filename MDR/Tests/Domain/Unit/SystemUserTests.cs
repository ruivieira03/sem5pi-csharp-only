using Xunit;
using Hospital.Domain.Users.SystemUser;

namespace Hospital.Tests.Domain.Unit{
    public class SystemUserTests{
        [Fact]
        public void AdminConstructor_ShouldInitializePropertiesCorrectly(){
            // Arrange
            var username = "adminUser";
            var role = Roles.Admin;
            var email = "admin@example.com";
            var phoneNumber = "123456789";
            var password = "securePassword";
            var iamId = "IAM123";

            // Act
            var user = new SystemUser(username, role, email, phoneNumber, password, iamId);

            // Assert
            Assert.Equal(username, user.Username);
            Assert.Equal(role, user.Role);
            Assert.Equal(email, user.Email);
            Assert.Equal(phoneNumber, user.PhoneNumber);
            Assert.Equal(password, user.Password);
            Assert.Equal(iamId, user.IAMId);
            Assert.False(user.isVerified);
        }

        [Fact]
        public void AdminConstructor_ShouldThrowException_ForPatientRole()
        {
            // Arrange & Act & Assert
            Assert.Throws<InvalidOperationException>(() => new SystemUser("patientUser", Roles.Patient, "patient@example.com", "987654321", "password", "IAM999"));
        }

        [Fact]
        public void Authenticate_ShouldReturnTrue_ForValidCredentials()
        {
            // Arrange
            var user = new SystemUser("testUser", Roles.Admin, "test@example.com", "12345", "password123", "IAM456");

            // Act
            var result = user.Authenticate("testUser", "IAM456");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Authenticate_ShouldReturnFalse_ForInvalidCredentials(){
            // Arrange
            var user = new SystemUser("testUser", Roles.Admin, "test@example.com", "12345", "password123", "IAM456");

            // Act
            var result = user.Authenticate("wrongUser", "IAM123");

            // Assert
            Assert.False(result);
        }
    }
}
