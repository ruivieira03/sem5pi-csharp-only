using Hospital.Domain.Patients;

namespace Hospital.Domain.Users.SystemUser
{
    public class SystemUserDto
    {
        public Guid Id { get; set; }               // Unique identifier for the user
        public string Username { get; set; }       // Username of the user
        public Roles Role { get; set; }            // Role (Admin, Doctor, Nurse, etc.)
        public string Email { get; set; }          // Email address
        public string PhoneNumber { get; set; }    // Phone number
        public string IAMId { get; set; }          // Identity and Access Management ID
        public string? ResetToken { get; set; }     // Token for password reset
        public DateTime? TokenExpiry { get; set; } // Expiry date for the reset token
        public bool isVerified { get; set; }       // Email verification status
        public string? VerifyToken { get; set; }   // Token for email verification
        public string? DeleteToken { get; set; }   // Token for account deletion
        public string? PatientId { get; set; }     // Unique identifier for the associated patient
        public Patient? Patient { get; set; }   // Associated patient details
    }
}
