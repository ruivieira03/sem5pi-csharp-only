namespace Hospital.Domain.Users.SystemUser{
    public class CreatingSystemUserDto{
        public string Username { get; set; }
        public Roles Role { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string IAMId { get; set; }

        public CreatingSystemUserDto(string Username, Roles Role, string Email, string PhoneNumber, string Password, string IAMId){
            this.Username = Username;
            this.Role = Role;
            this.Email = Email;
            this.PhoneNumber = PhoneNumber;
            this.Password = Password;
            this.IAMId = IAMId;
        }
    }
}
