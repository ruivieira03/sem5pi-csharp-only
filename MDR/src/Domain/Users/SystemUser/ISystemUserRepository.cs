
namespace Hospital.Domain.Users.SystemUser{
    public interface ISystemUserRepository{
        Task<SystemUser> GetByIdAsync(SystemUserId id); // Get user by Id
        Task<SystemUser> GetUserByUsernameAsync(string username); // Get user by Username
        Task<SystemUser> GetUserByEmailAsync(string email); // Get user by Email
        Task AddUserAsync(SystemUser user); // Add a new user
        Task UpdateUserAsync(SystemUser user); // Update an existing user
        Task Remove(SystemUser user); // Remove a user
        Task<List<SystemUser>> GetAllAsync(); // Get all users
    }
}
