using Moq;
using Xunit;
using Hospital.Services;

namespace Hospital.Tests.Domain.Unit
{
    public class EmailServiceTests
    {
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly EmailService _emailService;

        public EmailServiceTests()
        {
            _emailServiceMock = new Mock<IEmailService>();
            _emailService = new EmailService();
        }

        [Fact]
        public async Task SendRegistrationEmailAsync_ValidEmail_SendsEmail()
        {
            // Arrange
            string email = "ruimdv03@gmail.com"; 
            string setupLink = "https://localhost:5001/api/account/";

            // Mock the SendRegistrationEmailAsync to simulate email sending success
            _emailServiceMock.Setup(x => x.SendRegistrationEmailAsync(email, setupLink))
                             .Returns(Task.CompletedTask);  // No-op as if the email was sent successfully

            // Act & Assert
            var exception = await Record.ExceptionAsync(() => _emailService.SendRegistrationEmailAsync(email, setupLink));
            Assert.Null(exception); // Check that no exception is thrown
        }

        [Fact]
        public async Task SendPasswordResetEmailAsync_ValidEmail_SendsEmail()
        {
            // Arrange
            string email = "ruimdv03@gmail.com"; 
            string resetLink = "https://localhost:5001/api/account/";

            // Mock the SendPasswordResetEmailAsync to simulate email sending success
            _emailServiceMock.Setup(x => x.SendPasswordResetEmailAsync(email, resetLink))
                             .Returns(Task.CompletedTask);  // No-op as if the email was sent successfully

            // Act & Assert
            var exception = await Record.ExceptionAsync(() => _emailService.SendPasswordResetEmailAsync(email, resetLink));
            Assert.Null(exception); // Check that no exception is thrown
        }

        [Fact]
        public void GenerateSetupLink_ValidInputs_ReturnsCorrectLink()
        {
            // Arrange
            string email = "ruimdv03@gmail.com"; 
            string token = "12345";

            // Act
            string result = _emailService.GenerateSetupLink(email, token);

            // Assert
            Assert.Contains($"email={email}", result);
            Assert.Contains($"token={token}", result);
        }

        [Fact]
        public void GenerateResetLink_ValidInputs_ReturnsCorrectLink()
        {
            // Arrange
            string email = "ruimdv03@gmail.com"; 
            string token = "12345";

            // Act
            string result = _emailService.GenerateResetLink(email, token);

            // Assert
            Assert.Contains($"email={email}", result);
            Assert.Contains($"token={token}", result);
        }
    }
}
