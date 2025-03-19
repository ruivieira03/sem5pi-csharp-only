using System.ComponentModel.DataAnnotations;

namespace Hospital.ViewModels{
    public class GetUserViewModel{
        [Required(ErrorMessage = "Username is required.")]
        public required string Username { get; set; }

    }
}
