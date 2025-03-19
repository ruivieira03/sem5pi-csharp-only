using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Hospital.Domain.Patients;

namespace Hospital.ViewModels{   // Al us , regarding Patient Profile Here.
    public class RegisterPatientProfileViewModel{
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; } // Added FirstName

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; } // Added LastName

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; } // Added DateOfBirth

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; } // Added Gender
        
        [Required(ErrorMessage = "Email is required.")]             //Todo Verfiy Email tructure  format.
        [EmailAddress(ErrorMessage = "Invalid email format.")]  //update : Email format verified:         
        public string Email { get; set; } // Existing property

        [Required(ErrorMessage = "Phone number is required.")]    //Todo Verfiy Phonenuymber structure (9 Numberes only)
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } // Existing property

        [Required(ErrorMessage = "Emergency contact is required.")]
        public string EmergencyContact { get; set; } // Added EmergencyContact

        public List<string> AllergiesOrMedicalConditions { get; set; } = new List<string>(); // Added allergies/conditions
        public List<string>? AppointmentHistory { get; set; } = new List<string>(); // Added appointment history
    }
}