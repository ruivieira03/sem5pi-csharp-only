using System.ComponentModel.DataAnnotations;

namespace Hospital.ViewModels{
    public class PasswordResetViewModel{

        [Required(ErrorMessage = "New password is required.")]
        // Add proper ErrorMessage depending on the pwd criteria [StringLength(100, ErrorMessage = "Password must be at least {2} characters long.", MinimumLength = 6)]
        public required string Password { get; set; }
    }
}
