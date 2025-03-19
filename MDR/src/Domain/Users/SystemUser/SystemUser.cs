using Hospital.Domain.Shared;
using Hospital.Domain.Patients;

namespace Hospital.Domain.Users.SystemUser
{
    // Enum to represent different user roles in the system
    public enum Roles{
        Admin,
        Doctor,
        Staff,
        Patient
    }

    public class SystemUser : Entity<SystemUserId>, IAggregateRoot{
        // Public properties for EF Core to bind to
        public string Username { get; set; }       // Username of the user
        public Roles Role { get; set; }            // Role (Admin, Doctor, Nurse, etc.)
        public string Email { get; set; }          // Email address (embedded from ContactInformation)
        public string PhoneNumber { get; set; }    // Phone number (embedded from ContactInformation)
        public string Password { get; set; }        // Password of the user
        public string IAMId { get; set; }          // Unique ID linked to IAM (Identity and Access Management)
        public bool isVerified { get; set; } // For storing the email verification status
        public string? ResetToken { get; set; } // For storing the reset token

        public string? VerifyToken { get; set; } // For storing the verification token
        public string? DeleteToken { get; set; } // For storing the delete token
        public DateTime? TokenExpiry { get; set; } // For storing the token expiry time
        public Patient? Patient { get; set; } // Navigation property for patient
        public PatientId? PatientId { get; set; } // Foreign key for patientient 


        // Parameterless constructor for EF Core
        public SystemUser() {
            Id = new SystemUserId(Guid.NewGuid()); // Initialize Id here if needed
        }

        // Constructor for admin-registered users (backoffice staff)
        public SystemUser(string username, Roles role, string email, string phoneNumber, string password, string iamId){
            Id = new SystemUserId(Guid.NewGuid());
            Username = username;
            Role = role;
            Email = email;            // Set Email from the constructor
            PhoneNumber = phoneNumber; // Set PhoneNumber from the constructor
            Password = password;
            IAMId = iamId;

            isVerified = false; // Admin-registered users are automatically verified

            // Backoffice users are registered by admin and must not be patients
            if (Role == Roles.Patient){
                throw new InvalidOperationException("Patients must use the self-registration process.");
            }
        }

        // Constructor for self-registered patient users
        public SystemUser(string username, string email, string phoneNumber, string password, Patient patient){
            Id = new SystemUserId(Guid.NewGuid());
            Username = username;
            Role = Roles.Patient;     // Patients are self-registered
            Email = email;            // Set Email from the constructor
            PhoneNumber = phoneNumber; // Set PhoneNumber from the constructor
            Password = password;
            IAMId = Guid.NewGuid().ToString();

            isVerified = false; // Patients must verify their email before use

            // Associate the patient with the user
            Patient = patient;

        }

        // Simulate IAM authentication (would actually integrate with an external service)
        public bool Authenticate(string username, string iamId){
            return Username == username && IAMId == iamId;
        }

        public bool AuthenticateWithoutIAM(string username, string password){
            return Username == username && Password == password;
        }
    }
}
