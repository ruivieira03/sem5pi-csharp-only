
namespace Hospital.Services

{
public interface IPasswordService
    {
        string HashPassword(string password);
        bool ValidatePassword(string password, string hashedPassword);
        string GenerateTemporaryPassword(string username);
    }
}