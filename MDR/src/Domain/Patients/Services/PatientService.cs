using Hospital.Domain.Shared;
using Hospital.Services;
using Hospital.Domain.Users.SystemUser;
using Hospital.ViewModels;

namespace Hospital.Domain.Patients{
    public class PatientService{
        private readonly IPatientRepository _patientRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISystemUserRepository _systemUserRepository;
        private readonly ILoggingService _loggingService;
        private readonly IEmailService _emailService;

        public PatientService(IPatientRepository patientRepository, IUnitOfWork unitOfWork, ISystemUserRepository systemUserRepository, ILoggingService loggingService, IEmailService emailService){
            _patientRepository = patientRepository;
            _unitOfWork = unitOfWork;
            _systemUserRepository = systemUserRepository;
            _loggingService = loggingService;
            _emailService = emailService;
        }


    public async Task<PatientDto> UpdateProfileAsUserAsync(UpdateProfileViewModel model, SystemUserId userId){
        if (model == null){
            throw new ArgumentNullException(nameof(model), "Update model cannot be null.");
        }

         // Fetch the existing user
        var existingUser = await _systemUserRepository.GetByIdAsync(userId);
        if (existingUser == null){
            throw new InvalidOperationException("User not found.");
        }

        // Fetch the existing patient by user email
        var existingPatient = await _patientRepository.GetPatientByEmailAsync(existingUser.Email);
        if (existingPatient == null){
            throw new InvalidOperationException("Patient not found.");
        }

        // Log the start of the update process
        Console.WriteLine($"Starting update for patient {existingPatient.Id}, user {existingUser.Id}");

        // Store original data for comparison
        var originalPatientDto = new PatientDto{
            Id = existingPatient.Id.AsGuid(),
            FirstName = existingPatient.FirstName,
            LastName = existingPatient.LastName,
            Gender = existingPatient.Gender,
            Email = existingPatient.Email,
            PhoneNumber = existingPatient.PhoneNumber,
            EmergencyContact = existingPatient.EmergencyContact,
        
            AllergiesOrMedicalConditions = existingPatient.AllergiesOrMedicalConditions,
            AppointmentHistory = existingPatient.AppointmentHistory
        };

        // Update patient data only if the new data is non-null
        existingPatient.FirstName = model.FirstName ?? existingPatient.FirstName;
        existingPatient.LastName = model.LastName ?? existingPatient.LastName;
        existingPatient.Gender = model.Gender ?? existingPatient.Gender;

        if (!string.IsNullOrEmpty(model.Email)){
            existingPatient.Email = model.Email;
            existingUser.Email = model.Email;
        }

        if (!string.IsNullOrEmpty(model.PhoneNumber))
        {
            existingPatient.PhoneNumber = model.PhoneNumber;
            existingUser.PhoneNumber = model.PhoneNumber;
        }

        existingPatient.EmergencyContact = model.EmergencyContact ?? existingPatient.EmergencyContact;

        // Create a DTO for the updated patient
        var updatedPatientDto = new PatientDto{
             Id = existingPatient.Id.AsGuid(),
            FirstName = existingPatient.FirstName,
            LastName = existingPatient.LastName,
            Gender = existingPatient.Gender,
            Email = existingPatient.Email,
            PhoneNumber = existingPatient.PhoneNumber,
            EmergencyContact = existingPatient.EmergencyContact,
            AllergiesOrMedicalConditions = existingPatient.AllergiesOrMedicalConditions,
            AppointmentHistory = existingPatient.AppointmentHistory
        };

        // Compare changes and log
        try{
            string changedFields = _loggingService.GetChangedFields(originalPatientDto, updatedPatientDto);
            await _loggingService.LogProfileUpdateAsync(existingPatient.Id.ToString(), changedFields, DateTime.UtcNow);
        }
        catch (Exception logEx)
        {
            Console.WriteLine($"Logging failed: {logEx.Message}");
        }

        // If email or phone number changed, send verification
        if (originalPatientDto.Email != updatedPatientDto.Email || originalPatientDto.PhoneNumber != updatedPatientDto.PhoneNumber)
        {
            try
            {
                existingUser.VerifyToken = Guid.NewGuid().ToString();
                existingUser.TokenExpiry = DateTime.UtcNow.AddHours(48); // Token valid for 48 hours
                existingUser.isVerified = false;

                string setupLink = _emailService.GenerateEmailVerification(updatedPatientDto.Email, existingUser.VerifyToken);
                await _emailService.SendEmailConfirmationEmailAsync(updatedPatientDto.Email, setupLink);
            }
            catch (Exception emailEx)
            {
                Console.WriteLine($"Email verification failed: {emailEx.Message}");
                throw new InvalidOperationException("Failed to send email verification.");
            }
        }

        // Save updates to the repositories
        try{
            await _patientRepository.UpdatePatientAsync(existingPatient);
            await _systemUserRepository.UpdateUserAsync(existingUser);
            await _unitOfWork.CommitAsync();
        }

        catch (Exception dbEx){
            Console.WriteLine($"Database commit failed: {dbEx.Message}");
            throw new InvalidOperationException("Failed to save updates.");
        }

        // Return the updated patient data
        return updatedPatientDto;
    }


        // 5.8.7


        public async Task<PatientDto> UpdateProfileAsync(UpdatePatientProfileViewModel model, Guid patientId){
            
            if (model == null){
                throw new ArgumentNullException(nameof(model));
            }

            var existingPatient = await _patientRepository.GetByIdAsync(new PatientId(patientId));
           
            if (existingPatient == null){
                throw new InvalidOperationException("Patient not found.");
            }


            existingPatient.FirstName = model.FirstName;
            existingPatient.LastName = model.LastName;
            existingPatient.Email = model.Email;
            existingPatient.PhoneNumber = model.PhoneNumber;
            existingPatient.EmergencyContact = model.EmergencyContact;
            existingPatient.AllergiesOrMedicalConditions = model.AllergiesOrMedicalConditions;
            existingPatient.AppointmentHistory = model.AppointmentHistory;

            await _patientRepository.UpdatePatientAsync(existingPatient);   // Update Database
            await _unitOfWork.CommitAsync();                                // Commit transaction on it

            return new PatientDto{
                Id = existingPatient.Id.AsGuid(),
                FirstName = existingPatient.FirstName,
                LastName = existingPatient.LastName,
                DateOfBirth = existingPatient.DateOfBirth,
                Email = existingPatient.Email,
                Gender = existingPatient.Gender,
                MedicalRecordNumber = existingPatient.MedicalRecordNumber,
                PhoneNumber = existingPatient.PhoneNumber,
                EmergencyContact = existingPatient.EmergencyContact,
                AllergiesOrMedicalConditions = existingPatient.AllergiesOrMedicalConditions,
                AppointmentHistory = existingPatient.AppointmentHistory,
            };

        }


    public async Task <PatientDto> DeleteAsync(PatientId patientId){

            var existingPatient = await _patientRepository.GetByIdAsync(patientId);
            
            if (existingPatient == null){
                throw new InvalidOperationException("Patient not found.");
            }

            await _patientRepository.Remove(existingPatient);
            await _unitOfWork.CommitAsync();
             

            return new PatientDto{
           
           
                Id = existingPatient.Id.AsGuid(),
                FirstName = existingPatient.FirstName,
                LastName = existingPatient.LastName,
                DateOfBirth = existingPatient.DateOfBirth,
                Email = existingPatient.Email,
                Gender = existingPatient.Gender,
                MedicalRecordNumber = existingPatient.MedicalRecordNumber,
                PhoneNumber = existingPatient.PhoneNumber,
                EmergencyContact = existingPatient.EmergencyContact,
                AllergiesOrMedicalConditions = existingPatient.AllergiesOrMedicalConditions,
                AppointmentHistory = existingPatient.AppointmentHistory,

                
            };
        }
public async Task<PatientDto> DeletePersonalDataAsync(PatientId patientId)
{
    // Busca o paciente no repositório pelo ID
    var existingPatient = await _patientRepository.GetByIdAsync(patientId);

    if (existingPatient == null)
    {
        throw new InvalidOperationException("Patient not found.");
    }

    // Remove os dados pessoais do paciente
    await _patientRepository.RemovePersonalData(existingPatient);

    // Salva as alterações no banco de dados
    var changes = await _unitOfWork.CommitAsync();

    if (changes <= 0)
    {
        throw new InvalidOperationException("Failed to update patient data.");
    }

    // Retorna os dados do paciente (já atualizados) em forma de DTO
    return new PatientDto
    {
        Id = existingPatient.Id.AsGuid(),
        FirstName = existingPatient.FirstName,
        LastName = existingPatient.LastName,
        DateOfBirth = existingPatient.DateOfBirth,
        Email = existingPatient.Email,
        Gender = existingPatient.Gender,
        MedicalRecordNumber = existingPatient.MedicalRecordNumber,
        PhoneNumber = existingPatient.PhoneNumber,
        EmergencyContact = existingPatient.EmergencyContact,
        AllergiesOrMedicalConditions = existingPatient.AllergiesOrMedicalConditions,
        AppointmentHistory = existingPatient.AppointmentHistory
    };
}


        
    


       public async Task<List<PatientDto>> GetAllAsync(){
    var patients = await _patientRepository.GetAllAsync();
            List<PatientDto> patientDto= patients.ConvertAll(patient => new PatientDto { 

            Id = patient.Id.AsGuid(),
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            MedicalRecordNumber = patient.MedicalRecordNumber,
            Email = patient.Email,
            PhoneNumber = patient.PhoneNumber,
            EmergencyContact = patient.EmergencyContact,
            AllergiesOrMedicalConditions = patient.AllergiesOrMedicalConditions,            
            AppointmentHistory = patient.AppointmentHistory
            });

            
            return patientDto;
    }

           public async Task<PatientDto> GetByIdAsync(PatientId id){
            var patient = await this._patientRepository.GetByIdAsync(id);
            
            if (patient == null){
                throw new Exception("Patient not found.");
            }


            patient.FirstName = null;
            patient.LastName = null;
            patient.DateOfBirth = default; 
            patient.Email = null;
            patient.PhoneNumber = null;
            patient.Gender = null;
            patient.EmergencyContact = null;


         return new PatientDto { 

            Id = patient.Id.AsGuid(),
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            MedicalRecordNumber = patient.MedicalRecordNumber,
            Email = patient.Email,
            PhoneNumber = patient.PhoneNumber,
            EmergencyContact = patient.EmergencyContact,
            AllergiesOrMedicalConditions = patient.AllergiesOrMedicalConditions,            
            AppointmentHistory = patient.AppointmentHistory
            };
       
        }

          public async Task<PatientDto> GetByEmailAsync(string email){
        var patient = await this._patientRepository.GetPatientByEmailAsync(email);
            
            if (patient == null){
                throw new Exception("Patient not found.");
            }


         return new PatientDto { 

            Id = patient.Id.AsGuid(),
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            MedicalRecordNumber = patient.MedicalRecordNumber,
            Email = patient.Email,
            PhoneNumber = patient.PhoneNumber,
            EmergencyContact = patient.EmergencyContact,
            AllergiesOrMedicalConditions = patient.AllergiesOrMedicalConditions,            
            AppointmentHistory = patient.AppointmentHistory
            };
       
        }


        public async Task<PatientDto> GetByPhoneNumberAsync(string phoneNumber){
        var patient = await this._patientRepository.GetPatientByPhoneNumberAsync(phoneNumber);
            
            if (patient == null){
                throw new Exception("Patient not found.");
            }


         return new PatientDto { 

            Id = patient.Id.AsGuid(),
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            MedicalRecordNumber = patient.MedicalRecordNumber,
            Email = patient.Email,
            PhoneNumber = patient.PhoneNumber,
            EmergencyContact = patient.EmergencyContact,
            AllergiesOrMedicalConditions = patient.AllergiesOrMedicalConditions,            
            AppointmentHistory = patient.AppointmentHistory
            };
       
        }     

      public async Task<PatientDto> GetByFirstNameAsync(String firstName){
            var patient = await this._patientRepository.GetByFirstNameAsync(firstName);
            
            if (patient == null){
                throw new Exception("Patient not found.");
            }


         return new PatientDto { 

            Id = patient.Id.AsGuid(),
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            MedicalRecordNumber = patient.MedicalRecordNumber,
            Email = patient.Email,
            PhoneNumber = patient.PhoneNumber,
            EmergencyContact = patient.EmergencyContact,
            AllergiesOrMedicalConditions = patient.AllergiesOrMedicalConditions,            
            AppointmentHistory = patient.AppointmentHistory
            };
       
        }



      
}
}

    
