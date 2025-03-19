using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Hospital.Domain.Users.SystemUser;
using Hospital.Services;
using Hospital.ViewModels;
using Hospital.Domain.Patients;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase // Change to ControllerBase as it’s an API controller
{
    private readonly ISystemUserRepository _systemUserRepository;
    private readonly IPasswordService _passwordService;
    private readonly SystemUserService _systemUserService;
    private readonly PatientService _patientService;
    private readonly IConfiguration _configuration;

        public AccountController(
            ISystemUserRepository systemUserRepository,
            IPasswordService passwordService,
            SystemUserService systemUserService,
            PatientService patientService,
            IConfiguration configuration)
        {
            _systemUserRepository = systemUserRepository;
             _passwordService = passwordService;
            _systemUserService = systemUserService;
            _patientService = patientService;
             _configuration = configuration;
        }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestViewModel request)
    {
        // Log the configuration values
        var secretKey = _configuration["JwtSettings:SecretKey"];
        var issuer = _configuration["JwtSettings:Issuer"];
        var audience = _configuration["JwtSettings:Audience"];
        var expirationMinutes = _configuration["JwtSettings:ExpirationMinutes"];

        if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
        {
            return BadRequest("JWT configuration is missing or invalid.");
        }

        var user = await _systemUserRepository.GetUserByUsernameAsync(request.Username);

        if (user == null)
        {
            return Unauthorized("Invalid username.");
        }

         // Hash the password and verify
        string hashedPassword = _passwordService.HashPassword(request.Password);
        if (!user.AuthenticateWithoutIAM(request.Username, hashedPassword))
            return Unauthorized("Invalid username or password.");

        // Create claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString()), // Role claim
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.Value.ToString())
        };

        // Configure JWT token settings
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(expirationMinutes)),
            signingCredentials: creds
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.WriteToken(token);

        // Return both the JWT token and the role
        return Ok(new 
        {
            token = jwtToken,
            role = user.Role.ToString()  // Include role in the response
        });
    }

    [Authorize]
    [HttpGet("profile")]
    public IActionResult Profile()
    {
        var username = User.Identity.Name;
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        return Ok(new
        {
            Username = username,
            Email = email,
            Role = role,
            UserId = userId
        });
    }


    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Just a message indicating successful logout on the server side.
        // Actual logout is handled on the client side by removing the token.
        return Ok(new { message = "Logged out successfully." });
    }


    [HttpGet("setup-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateToken()
    {
        string email = Request.Query["email"].ToString();
        string token = Request.Query["token"].ToString();

        bool isValid = await _systemUserService.ValidateTokenForUser(email, token);
        if (!isValid)
        {
            return BadRequest(new { message = "Invalid token." });
        }
        return Ok(new { message = "Token is valid." });
    }

    [HttpPost("request-password-reset")]
    [AllowAnonymous]
    public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestViewModel model)
    {
        try
        {
            await _systemUserService.RequestPasswordResetAsync(model.Email);
            return Ok(new { message = "Password reset link has been sent to your email." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("reset-password")]
    [AllowAnonymous]
    public IActionResult RedirectToResetPasswordPage()
    {
        string email = Request.Query["email"].ToString();
        string token = Request.Query["token"].ToString();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new { message = "Missing email or token in query parameters." });
        }

         // Build the frontend reset-password URL without encoding the email
        var frontendUrl = $"http://localhost:3000/reset-password?email={email}&token={token}";

        // Redirect the user to the frontend
        return Redirect(frontendUrl);
    }


    // Handle the password reset logic
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] PasswordResetViewModel model)
    {
        string email = Request.Query["email"].ToString();
        string token = Request.Query["token"].ToString();

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _systemUserService.ResetPasswordAsync(email, model.Password, token);
            return Ok(new { message = "Password has been reset successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("redirect-confirm-email")]
    [AllowAnonymous]
    public IActionResult RedirectToConfirmEmailPage()
    {
        string email = Request.Query["email"].ToString();
        string token = Request.Query["token"].ToString();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new { message = "Missing email or token in query parameters." });
        }

         // Build the frontend reset-password URL without encoding the email
        var frontendUrl = $"http://localhost:3000/confirm-email?email={email}&token={token}";

        // Redirect the user to the frontend
        return Redirect(frontendUrl);
    }

    [HttpGet("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail()
    {
        string email = Request.Query["email"].ToString();
        string token = Request.Query["token"].ToString();


        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
        {
            return BadRequest(new { message = "Email or token is missing." });
        }

        try
        {
            bool isValid = await _systemUserService.ValidateEmailTokenAsync(email, token);
            if (isValid)
            {
                bool res = await _systemUserService.ConfirmEmailAsync(email, token);
                if (res)
                {
                    return Ok(new { message = "Email confirmed successfully." });
                }
                else
                {
                    return BadRequest(new { message = "Email confirmation failed." });
                }
            }
            else
            {
                return BadRequest(new { message = "Invalid token or email confirmation failed." });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("request-delete-account")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> RequestDeleteAccount()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID is missing.");
        }

        try
        {
            await _systemUserService.RequestAccountDeletionAsync(new SystemUserId(userId));
            return Ok(new { message = "A confirmation email has been sent to your registered email address." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("redirect-delete-account")]
    [AllowAnonymous]
    public IActionResult RedirectToConfirmDeleteAccountPage()
    {
        string email = Request.Query["email"].ToString();
        string token = Request.Query["token"].ToString();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new { message = "Missing email or token in query parameters." });
        }

         // Build the frontend reset-password URL without encoding the email
        var frontendUrl = $"http://localhost:3000/delete-account?email={email}&token={token}";

        // Redirect the user to the frontend
        return Redirect(frontendUrl);
    }
    
    [HttpGet("delete-account")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmDeleteAccount()
    {
        string email = Request.Query["email"].ToString();
        string token = Request.Query["token"].ToString();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
        {
            return BadRequest(new { message = "Email or token is missing." });
        }

        try
        {
            bool isValid = await _systemUserService.ValidateDeleteTokenAsync(email, token);
            if (isValid)
            {
                await _systemUserService.DeleteAsync(email);
            
                return Ok(new { message = "Account deleted successfully." });
            }
            else
            {
                return BadRequest(new { message = "Invalid token or account deletion failed." });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("patient-profile")]
    [Authorize] // Ensure the user is authenticated
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the user ID from claims

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(); // Return 401 if user ID is not found
        }

        var user = await _systemUserRepository.GetByIdAsync(new SystemUserId(userId)); // Fetch the user

        if (user == null)
        {
            return NotFound(new { message = "User not found." }); // Return 404 if user not found
        }

        var patientId = user.PatientId; // Get the patient ID from the user

        if (patientId == null)
        {
            return NotFound(new { message = "Patient not found." }); // Return 404 if patient ID is not found
        }

        try
        {
            var patientProfile = await _patientService.GetByIdAsync(patientId); // Fetch the patient profile
            if (patientProfile == null)
            {
                return NotFound(new { message = "Patient not found." }); // Return 404 if patient not found
            }

            return Ok(patientProfile); // Return the patient profile
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message }); // Return 400 for any other errors
        }
    }

    [HttpPut("update-profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updatedPatient = await _patientService.UpdateProfileAsUserAsync(model, new SystemUserId(userId));
            return Ok(updatedPatient);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}