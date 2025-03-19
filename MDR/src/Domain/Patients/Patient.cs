using Hospital.Domain.Shared;
using System.ComponentModel.DataAnnotations.Schema;
using Hospital.Domain.Users.SystemUser;

namespace Hospital.Domain.Patients{
    public class Patient : Entity<PatientId>{
        public string FirstName { get; set; }                        // First name of the patient
        public string LastName { get; set; }                         // Last name of the patient
       
        [NotMapped]  // hmm , for later, what does this mean ?
        public string FullName => $"{FirstName} {LastName}";        // Full name, derived from first and last name
        public DateTime DateOfBirth { get; set; }                   // Date of birth of the patient
        public string Gender { get; set; }                           // Gender of the patient
        public string MedicalRecordNumber { get; set; }             // Unique identifier for the patient's medical record
        public string Email { get; set; }                            // Email address of the patient
        public string PhoneNumber { get; set; }                      // Phone number of the patient
        public List<string> AllergiesOrMedicalConditions { get; set; } // Optional list of allergies or medical conditions
        public string EmergencyContact { get; set; }                 // Emergency contact information
        public List<string>? AppointmentHistory { get; set; }    // List of previous and upcoming appointments
        public SystemUser? SystemUser { get; set; } // Navigation property back to SystemUser

    


        // Parameterless constructor for EF Core
        public Patient(){
            Id = new PatientId(Guid.NewGuid()); // Initialize Id here if needed
            AppointmentHistory = new List<string>();
            AllergiesOrMedicalConditions = new List<string>();

        }

        // Constructor to create a new patient with necessary details
        public Patient(string firstName, string lastName, DateTime dateOfBirth, string gender,
                       string medicalRecordNumber, string email, string phoneNumber, string emergencyContact, List<string> appointmentHistory, List<string> allergiesOrMedicalConditions){
            Id = new PatientId(Guid.NewGuid()); // Generate a new unique ID == guid // Duvida prof Eapli//Arqsi Aqui ou no serviço.
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            MedicalRecordNumber = medicalRecordNumber;
            Email = email;                                  // Email must be unique
            PhoneNumber = phoneNumber;                      
            EmergencyContact = emergencyContact;
            AppointmentHistory = appointmentHistory;
            AllergiesOrMedicalConditions = allergiesOrMedicalConditions;
        
        }

        // Method to update patient profile details
        public void UpdateProfile(string firstName, string lastName, string email, string phoneNumber, string emergencyContact){
            
            FirstName = firstName;
            LastName = lastName;
            Email = email;                              // Email can trigger additional verification if changed
            PhoneNumber = phoneNumber;                  // Phone can trigger additional verification if changed
            EmergencyContact = emergencyContact;
        }
        
    }



}
