
namespace Hospital.Services{
    public interface IEmailService{
        Task SendRegistrationEmailAsync(string email, string setupLink);
        Task SendPasswordResetEmailAsync(string email, string setupLink);
        Task SendEmailConfirmationEmailAsync(string email, string setupLink);
        Task SendAccountDeletionEmailAsync(string email, string setupLink);
        string GenerateDeleteLink(string email, string token);
        string GenerateSetupLink(string email, string token);
        string GenerateResetLink(string email, string token);
        string GenerateEmailVerification(string email, string token);
        
    }
}
