namespace Hospital.Domain.Shared{
    public class ContactInformation{
        public int Id { get; set; } // Primary key

        public string Email { get; set; } // Email address
        public string PhoneNumber { get; set; } // Phone number

        // Parameterless constructor for EF Core
        public ContactInformation() { }

        // Constructor with parameters for easy instantiation
        public ContactInformation(string email, string phoneNumber){
            Email = email;
            PhoneNumber = phoneNumber;
        }
    }
}
