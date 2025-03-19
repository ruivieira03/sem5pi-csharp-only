using Hospital.Domain.Users.SystemUser;

namespace Hospital.Domain.Patients
{
    public class PatientDto{
        public Guid Id { get; set; }                                 // Unique identifier for the patient
        public string FirstName { get; set; }                        // First name of the patient
        public string LastName { get; set; }                         // Last name of the patient
        public string FullName => $"{FirstName} {LastName}";        // Full name, derived from first and last name
        public DateTime DateOfBirth { get; set; }                   // Date of birth of the patient
        public string Gender { get; set; }                           // Gender of the patient
        public string MedicalRecordNumber { get; set; }             // Unique identifier for the patient's medical record
        public string Email { get; set; }                            // Email address of the patient
        public string PhoneNumber { get; set; }                      // Phone number of the patient
        public List<string> AllergiesOrMedicalConditions { get; set; } // Optional list of allergies or medical conditions
        public string EmergencyContact { get; set; }                 // Emergency contact information
        public SystemUserId? SystemUserId { get; set; }               // Unique identifier for the system user
        public SystemUser? SystemUser { get; set; }                // System user associated with the patient


        public List<string>? AppointmentHistory {get;set;}      // List of apointment

    }
}
