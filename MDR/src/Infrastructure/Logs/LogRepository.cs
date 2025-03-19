using Hospital.Domain.Logs;
using Hospital.Infrastructure;

namespace Hospital.Infrastructure.Logs
{
    public class LogRepository : ILogRepository
    {
        private readonly HospitalDbContext _context;

        public LogRepository(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task LogProfileUpdateAsync(ProfileUpdateLog logEntry)
        {
            _context.ProfileUpdateLogs.AddAsync(logEntry);
        }

        public async Task LogAccountDeletionAsync(AccountDeletionLog logEntry)
        {
            _context.AccountDeletionLogs.AddAsync(logEntry);
        }
    }
}
