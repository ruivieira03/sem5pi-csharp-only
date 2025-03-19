using System.Threading.Tasks;
using Hospital.Domain.Patients;

namespace Hospital.Services
{
    public interface ILoggingService
    
    {
        Task LogProfileUpdateAsync(string userId, string changedFields, DateTime timestamp);
        Task LogAccountDeletionAsync(string userId, DateTime timestamp);
        string GetChangedFields(PatientDto existingPatient, PatientDto updatedPatient);
        string GetChangedFields(Patient existingPatient, PatientDto editedPatient);
    }
}
