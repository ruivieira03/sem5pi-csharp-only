using System.ComponentModel.DataAnnotations;

namespace Hospital.ViewModels{
    public class LoginRequestViewModel{
        [Required(ErrorMessage = "Username is required.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public required string Password { get; set; }
    }
}
