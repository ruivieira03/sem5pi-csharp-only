using Microsoft.AspNetCore.Mvc;
using Hospital.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Hospital.Domain.Users.SystemUser;
using Hospital.Domain.Patients;
using System.Security.Claims; // Para ClaimTypes
using Hospital.Domain.Patients; // Para PatientId

//using  org.springframework.hateoas.RepresentationModel;

namespace Hospital.Controllers{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase{
        private readonly PatientRegistrationService _patientRegistrationService;
        private readonly ISystemUserRepository _systemUserRepository;
        private readonly SystemUserService _systemUserService;
        private readonly PatientService _patientService;

        public PatientController(PatientRegistrationService patientRegistrationService, ISystemUserRepository systemUserRepository, PatientService patientService){
            _patientRegistrationService = patientRegistrationService;
            _systemUserRepository = systemUserRepository;
            _patientService = patientService;
        }

        // POST api/patient/register-profile  
        //@RestController  
        [HttpPost("register-profile")]
        [Authorize(Roles = "Admin")]
       // @AutoWired
         public async Task<IActionResult> RegisterPatientProfile([FromBody] RegisterPatientProfileViewModel model){

            // Check if all ViewModel Inputs the model state is valid
            if (!ModelState.IsValid){
                return BadRequest(ModelState); // 400 
            }

            try{

                var newPatientDto = await _patientRegistrationService.RegisterPatientProfileAsync(model); // Delegate the user registration Logic to the service layer
                return CreatedAtAction(nameof(RegisterPatientProfile), new {id = newPatientDto.Id },
                 
                 new {
                    message = "Patient profile created successfully", // Sucess message
                    patient = newPatientDto                           // return message and patients
                });
                
            }catch (Exception ex){
                return BadRequest(new { message = ex.Message, innerException = ex.InnerException?.Message });// 
            }

        }

        

        // PUT: api/Patient/5/update-profile Update the patient's profile details
        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProfile([FromRoute] Guid id, UpdatePatientProfileViewModel model){
        Console.WriteLine("\n\nATE AQUI TUDO BEM\n\n");
        Console.WriteLine("\n\n" + id + "\n\n");
        
            if (!ModelState.IsValid){
                return BadRequest(ModelState);
            }


            try {
                var updatedPatient = await _patientService.UpdateProfileAsync(model, id);   // Delegate the update logic to the service layer
                return Ok(updatedPatient);                                                  // Return OK with the updated user

            }catch (Exception ex){
                return BadRequest(new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }

   [HttpDelete("delete-personal-data/{id}")]
[Authorize(Roles = "Patient")]
public async Task<ActionResult<PatientDto>> DeletePersonalData(Guid id)
{
    try
    {
        // Chama o serviço para remover os dados pessoais
        var patient = await _patientService.DeletePersonalDataAsync(new PatientId(id));

        if (patient == null)
        {
            return NotFound(); // Retorna 404 se o paciente não for encontrado
        }

        return Ok(patient); // Retorna 200 com os dados atualizados do paciente
    }
    catch (Exception ex)
    {
        // Retorna 400 com mensagem de erro
        return BadRequest(new { Message = ex.Message });
    }
}





              
 // DELETE: api/Patient/5/Delete-Profilea 
    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PatientDto>> DeletePatientProfile(Guid id){

        try{
           var patient =  await _patientService.DeleteAsync(new PatientId(id));


            if (patient == null){
                return NotFound(); // Return 404 if user not found
            }

            return Ok(patient); // Return OK with the deleted user's details
        }catch (Exception ex){
            return BadRequest(new { Message = ex.Message }); // Return 400 if any business rule fails
        }
         
    }


        
// GET: api/patient/getall
[HttpGet("getAll")] 
[Authorize(Roles = "Admin")] 
public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll(){
        var patient = await _patientService.GetAllAsync();
            return Ok(patient); // Return OK status with the list of users
            
        }

// GET: api/Patient/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PatientDto>> GetById(Guid id){

        var Patient = await _patientService.GetByIdAsync(new PatientId(id));

        if (Patient == null){
            return NotFound(); // Return 404 if user not found
        }

        return Ok(Patient); // Return OK status with the user data
    }


// GET: api/Patient/{email}
    [HttpGet("email/{email}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PatientDto>> GetByEmail(string email){

        var patient = await _patientService.GetByEmailAsync(email);

        if (patient == null){
            return NotFound(); // Return 404 if user not found
        }

        return Ok(patient); // Return OK status with the user data
    }

    // GET: api/Patient/{email}
    [HttpGet("phoneNumber/{phoneNumber}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PatientDto>> GetByPhoneNumberAsync(string phoneNumber){

        var Patient = await _patientService.GetByPhoneNumberAsync(phoneNumber);

        if (Patient == null){
            return NotFound(); // Return 404 if user not found
        }

        return Ok(Patient); // Return OK status with the user data
    }




    [HttpGet("firstName/{firstname}")]
    [Authorize(Roles = "Admin")]
public async Task<ActionResult<IEnumerable<PatientDto>>> GetByFirstName(string firstName){
    
    if (string.IsNullOrEmpty(firstName)){
        return BadRequest("First name is required.");
    }

    try{
        var patients = await _patientService.GetByFirstNameAsync(firstName);
        return Ok(patients);
    }catch (Exception ex){
        return BadRequest(new { message = ex.Message });
    }
}

}
    
        
}
