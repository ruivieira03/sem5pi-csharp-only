using Hospital.Domain.Users.SystemUser;
using System.ComponentModel.DataAnnotations;

namespace Hospital.ViewModels
{
    public class UpdateSystemUserViewModel
    {

        [MaxLength(50)]
        public string? Username { get; set; } // Updated username

        public Roles? Role { get; set; } // Updated role of the user

        [EmailAddress]
        public string? Email { get; set; } // Updated email address

        [Phone]
        public string? PhoneNumber { get; set; } // Updated phone number

    }
}
