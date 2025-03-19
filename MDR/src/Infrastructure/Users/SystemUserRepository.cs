using Microsoft.EntityFrameworkCore;
using Hospital.Domain.Users.SystemUser;

namespace Hospital.Infrastructure.Users
{
    public class SystemUserRepository : ISystemUserRepository
    {
        private readonly HospitalDbContext _context;

        public SystemUserRepository(HospitalDbContext context)
        {
            _context = context;
        }

        // Fetch a user by their Id
        public async Task<SystemUser> GetByIdAsync(SystemUserId id)
        {
            // Pass the internal Guid of the SystemUserId to FindAsync
            return await _context.SystemUsers.FirstOrDefaultAsync(user => user.Id == id);
        }


        // Fetch a user by their Username
        public async Task<SystemUser> GetUserByUsernameAsync(string username)
        {
            return await _context.SystemUsers.FirstOrDefaultAsync(user => user.Username == username);
        }

        // Fetch a user by their Email
        public async Task<SystemUser> GetUserByEmailAsync(string email)
        {
            return await _context.SystemUsers.FirstOrDefaultAsync(user => user.Email == email);
        }

        // Add a new user to the database
        public async Task AddUserAsync(SystemUser user)
        {
            _context.SystemUsers.AddAsync(user);
        }

        // Update an existing user in the database
        public async Task UpdateUserAsync(SystemUser user)
        {
            _context.SystemUsers.Update(user);
        }

        // Remove a user from the database
        public async Task Remove(SystemUser user)
        {
            _context.SystemUsers.Remove(user); // Direct removal using Entity Framework
        }

        // Fetch all users from the database
        public async Task<List<SystemUser>> GetAllAsync()
        {
            return await _context.SystemUsers.ToListAsync();
        }
    }
}
