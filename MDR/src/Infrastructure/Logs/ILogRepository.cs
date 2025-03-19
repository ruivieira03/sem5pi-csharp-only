using Hospital.Domain.Logs;

namespace Hospital.Infrastructure.Logs

{
    public interface ILogRepository
    {
        Task LogProfileUpdateAsync(ProfileUpdateLog logEntry);
        Task LogAccountDeletionAsync(AccountDeletionLog logEntry);
    }
}