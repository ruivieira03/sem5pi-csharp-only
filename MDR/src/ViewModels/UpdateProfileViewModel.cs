using System.ComponentModel.DataAnnotations;

public class UpdateProfileViewModel{
    // Optional first name of the patient
    [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
    public string FirstName { get; set; }

    // Optional last name of the patient
    [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
    public string LastName { get; set; }

    // Optional gender of the patient
    [MaxLength(10, ErrorMessage = "Gender cannot exceed 10 characters.")]
    public string Gender { get; set; }

    // Optional email address of the patient
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; }

    // Optional phone number of the patient
    [Phone(ErrorMessage = "Invalid phone number format.")]
    public string PhoneNumber { get; set; }

    // Optional emergency contact information
    [MaxLength(100, ErrorMessage = "Emergency contact information cannot exceed 100 characters.")]
    public string EmergencyContact { get; set; }

}
