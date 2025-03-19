using System;
using System.Security.Cryptography;
using System.Text;

namespace Hospital.Services
{
    public class PasswordService : IPasswordService
    {
        
        public string HashPassword(string password)
        {
            // Hashing the password using SHA256
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public bool ValidatePassword(string password, string hashedPassword)
        {
            // Hash the input password and compare it with the hashed password
            return HashPassword(password) == hashedPassword;
        }

        public string GenerateTemporaryPassword(string username)
        {
            // Ensure the username is not null or empty
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            }

            // Generate the temporary password by concatenating the username with "1234"
            var temporaryPassword = username + "1234";

            return temporaryPassword;
        }

    }
}
