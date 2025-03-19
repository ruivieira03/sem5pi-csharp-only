using Hospital.ViewModels;
using Hospital.Domain.Shared;
using Hospital.Services;
using Hospital.Domain.Patients;

namespace Hospital.Domain.Users.SystemUser
{
    public class SystemUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISystemUserRepository _systemUserRepository;
        private readonly IPasswordService _passwordService;
        private readonly IEmailService _emailService;
        private readonly IPatientRepository _patientRepository;
        private readonly ILoggingService _loggingService;

        public SystemUserService(IUnitOfWork unitOfWork, ISystemUserRepository systemUserRepository, IPasswordService passwordService, IEmailService emailService, IPatientRepository patientRepository, ILoggingService loggingService)
        {
            this._unitOfWork = unitOfWork;
            this._systemUserRepository = systemUserRepository;
            this._passwordService = passwordService;
            this._emailService = emailService;
            this._patientRepository = patientRepository;
            this._loggingService = loggingService;
        }

        public async Task<SystemUserDto> RegisterUserAsync(RegisterUserViewModel model)
        {
            // Verify if the username is already taken

            if (await _systemUserRepository.GetUserByUsernameAsync(model.Username) != null)
            {
                throw new Exception("Username already taken.");
            }

            if (await _systemUserRepository.GetUserByEmailAsync(model.Email) != null)
            {
                throw new Exception("Email already taken.");
            }

            // Generate a temporary password
            string temporaryPassword = _passwordService.GenerateTemporaryPassword(model.Username);

            // Hash the temporary password before saving it
            string hashedPassword = _passwordService.HashPassword(temporaryPassword);

            // Create a new SystemUser with the hashed password
            var newUser = new SystemUser(
                model.Username, 
                model.Role, 
                model.Email, 
                model.PhoneNumber, 
                hashedPassword, 
                Guid.NewGuid().ToString()
            );

            // Generate and store the reset token
            newUser.VerifyToken = Guid.NewGuid().ToString();
            newUser.TokenExpiry = DateTime.UtcNow.AddHours(24); // Token valid for 24 hours

            // Save the user to the repository
            await _systemUserRepository.AddUserAsync(newUser);

            // Generate a one-time setup link
            string setupLink = _emailService.GenerateSetupLink(model.Email, newUser.VerifyToken);

            // Send the registration email with the setup link
            await _emailService.SendRegistrationEmailAsync(newUser.Email, setupLink);

            // Commit the transaction
            await _unitOfWork.CommitAsync();

            // Return a DTO with the new user’s details
            return new SystemUserDto
            {
                Id = newUser.Id.AsGuid(),
                Username = newUser.Username,
                Role = newUser.Role,
                Email = newUser.Email,
                PhoneNumber = newUser.PhoneNumber,
                IAMId = newUser.IAMId,
                ResetToken = newUser.ResetToken,
                TokenExpiry = newUser.TokenExpiry,
                isVerified = newUser.isVerified,
                VerifyToken = newUser.VerifyToken
            };
        }

        // Register a new patient user
        // This method is similar to the RegisterUserAsync method, but it also links the patient profile to the new user
        public async Task<SystemUserDto> RegisterPatientUserAsync(PatientUserViewModel model)
        {
            // Verify if the username and email is already taken

            if (await _systemUserRepository.GetUserByUsernameAsync(model.Username) != null)
            {
                throw new Exception("Username already taken.");
            }

            if (await _systemUserRepository.GetUserByEmailAsync(model.Email) != null)
            {
                throw new Exception("Email already taken.");
            }

            // Hash the temporary password before saving it
            string hashedPassword = _passwordService.HashPassword(model.Password);

            var patientProfile = await _patientRepository.GetPatientByEmailAsync(model.Email);

            if (patientProfile == null) 
            {
                throw new Exception("Patient profile for that email doesn't exist.");
            }

            // Create a new SystemUser with the hashed password
            var newUser = new SystemUser(
                model.Username, 
                model.Email, 
                model.PhoneNumber, 
                hashedPassword,
                patientProfile
            );

            // Generate and store the reset token
            newUser.VerifyToken = Guid.NewGuid().ToString();
            newUser.TokenExpiry = DateTime.UtcNow.AddHours(24); // Token valid for 24 hours

            // Save the user to the repository
            await _systemUserRepository.AddUserAsync(newUser);

            // Generate a one-time setup link
            string setupLink = _emailService.GenerateEmailVerification(model.Email, newUser.VerifyToken);

            // Send the registration email with the setup link
            await _emailService.SendRegistrationEmailAsync(newUser.Email, setupLink);

            // Commit the transaction
            await _unitOfWork.CommitAsync();

            // Return a DTO with the new user’s details
            return new SystemUserDto
            {
                Id = newUser.Id.AsGuid(),
                Username = newUser.Username,
                Role = newUser.Role,
                Email = newUser.Email,
                PhoneNumber = newUser.PhoneNumber,
                IAMId = newUser.IAMId,
                TokenExpiry = newUser.TokenExpiry,
                isVerified = newUser.isVerified,
                VerifyToken = newUser.VerifyToken
            };
        }


        // Request password reset by generating a token and sending an email
        public async Task<SystemUserDto> RequestPasswordResetAsync(string email)
        {
            var user = await _systemUserRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Generate reset token and set expiry time
            user.ResetToken = Guid.NewGuid().ToString();
            user.TokenExpiry = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour

            // Generate reset link
            string resetLink = _emailService.GenerateResetLink(user.Email, user.ResetToken);
            await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);

            // Commit the transaction
            await _unitOfWork.CommitAsync();

            return new SystemUserDto
            {
                Id = user.Id.AsGuid(),
                Username = user.Username,
                Role = user.Role,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IAMId = user.IAMId,
                ResetToken = user.ResetToken,
                isVerified = user.isVerified,
                VerifyToken = user.VerifyToken
            };
        }

        // Reset password using the reset token

        public async Task<SystemUserDto> ResetPasswordAsync(string email, string newPassword, string token)
        {
            var user = await _systemUserRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            
            // Check if the token is valid
            bool isValidToken = await ValidateTokenForUser(email, token);
            if (!isValidToken)
            {
                throw new Exception("Invalid or expired reset token.");
            }

            // Hash and update the new password
            user.Password = _passwordService.HashPassword(newPassword);
            user.ResetToken = null;  // Clear reset token after use
            user.TokenExpiry = null;

            await _unitOfWork.CommitAsync();

            return new SystemUserDto{
                Id = user.Id.AsGuid(),
                Username = user.Username,
                Role = user.Role,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IAMId = user.IAMId,
                ResetToken = user.ResetToken,
                TokenExpiry = user.TokenExpiry,
                isVerified = user.isVerified
            };
        }


        // Fetch all users
        public async Task<List<SystemUserDto>> GetAllAsync()
        {
            var users = await this._systemUserRepository.GetAllAsync();
            
            List<SystemUserDto> userDtos = users.ConvertAll(user => new SystemUserDto
            {
                Id = user.Id.AsGuid(),
                Username = user.Username,
                Role = user.Role,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IAMId = user.IAMId,
                isVerified = user.isVerified,
                VerifyToken = user.VerifyToken,
                ResetToken = user.ResetToken,
                TokenExpiry = user.TokenExpiry,
                DeleteToken = user.DeleteToken
            });

            return userDtos;
        }

        // Fetch user by Id
        public async Task<SystemUserDto> GetByIdAsync(SystemUserId id)
        {
            var user = await this._systemUserRepository.GetByIdAsync(id);
            
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return new SystemUserDto
            {
                Id = user.Id.AsGuid(),
                Username = user.Username,
                Role = user.Role,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IAMId = user.IAMId,
                isVerified = user.isVerified,
                VerifyToken = user.VerifyToken,
                ResetToken = user.ResetToken,
                TokenExpiry = user.TokenExpiry,
                DeleteToken = user.DeleteToken
            };
        }

        // Fetch user by Id
        public async Task<SystemUserDto> GetByUsernameAsync(string username)
        {
            var user = await this._systemUserRepository.GetUserByUsernameAsync(username);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return new SystemUserDto
            {
                Id = user.Id.AsGuid(),
                Username = user.Username,
                Role = user.Role,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IAMId = user.IAMId,
                isVerified = user.isVerified,
                VerifyToken = user.VerifyToken,
                ResetToken = user.ResetToken,
                TokenExpiry = user.TokenExpiry,
                DeleteToken = user.DeleteToken
            };
        }

        // Update user
        public async Task<SystemUserDto> UpdateAsync(Guid id, UpdateSystemUserViewModel model)
        {
            var user = await this._systemUserRepository.GetByIdAsync(new SystemUserId(id));

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Check if the username is already taken by another user
            if (model.Username != null && 
                (await this._systemUserRepository.GetUserByUsernameAsync(model.Username)) is { } existingUserByUsername &&
                        existingUserByUsername.Id.AsGuid() != id)
            {
                throw new Exception("Username is already in use by another user.");
            }

            // Check if the email is already taken by another user
            if (model.Email != null && 
                (await this._systemUserRepository.GetUserByEmailAsync(model.Email)) is { } existingUserByEmail &&
                existingUserByEmail.Id.AsGuid() != id)
            {
                throw new Exception("Email is already in use by another user.");
            }

            string originalEmail = user.Email;

            // Update user details
            if (model.Username != null)
            {
                user.Username = model.Username;
            }

            if (model.Role != null)
            {
                user.Role = (Roles)model.Role;
            }

            if (model.Email != null)
            {
                user.Email = model.Email;
            }

            if (model.PhoneNumber != null)
            {
                user.PhoneNumber = model.PhoneNumber;
            }

            // Handle email verification if the email has changed
            if (originalEmail != model.Email && model.Email != null) 
            {
                // Generate and store the verification token
                user.VerifyToken = Guid.NewGuid().ToString();
                user.TokenExpiry = DateTime.UtcNow.AddHours(48); // Token valid for 48 hours
                user.isVerified = false;

                string setupLink = _emailService.GenerateEmailVerification(model.Email, user.VerifyToken);

                await _emailService.SendEmailConfirmationEmailAsync(model.Email, setupLink);
            }

            await this._unitOfWork.CommitAsync();

            return new SystemUserDto
            {
                Id = user.Id.AsGuid(),
                Username = user.Username,
                Role = user.Role,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IAMId = user.IAMId,
                isVerified = user.isVerified,
                VerifyToken = user.VerifyToken,
                ResetToken = user.ResetToken,
                TokenExpiry = user.TokenExpiry,
                DeleteToken = user.DeleteToken
            };
        }


        // Inactivate user
        public async Task<SystemUserDto> InactivateAsync(SystemUserId id)
        {
            var user = await this._systemUserRepository.GetByIdAsync(id); 

            if (user == null)
            {
                throw new Exception("User not found.");
            }   

            // Inactivate user (mark as inactive, or adjust the field you use to signify activity)
            user.ResetToken = null; // #TODO: inactivate user logic

            await this._unitOfWork.CommitAsync();

            return new SystemUserDto
            {
                Id = user.Id.AsGuid(),
                Username = user.Username,
                Role = user.Role,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IAMId = user.IAMId,
                isVerified = user.isVerified,
                VerifyToken = user.VerifyToken,
                ResetToken = user.ResetToken,
                TokenExpiry = user.TokenExpiry,
                DeleteToken = user.DeleteToken
            };
        }

        // Delete user
        public async Task<SystemUserDto> DeleteAsync(string email)
        {
            var user = await this._systemUserRepository.GetUserByEmailAsync(email); 

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            await this._systemUserRepository.Remove(user);
            
            if (user.Id != null)
            {
                if (user.Id != null)
                {
                    await this._loggingService.LogAccountDeletionAsync(user.Id.ToString(), DateTime.UtcNow);
                }
            }

            await this._unitOfWork.CommitAsync();

            return new SystemUserDto
            {
                Id = user.Id.AsGuid(),
                Username = user.Username,
                Role = user.Role,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IAMId = user.IAMId,
                isVerified = user.isVerified,
                VerifyToken = user.VerifyToken,
                ResetToken = user.ResetToken,
                TokenExpiry = user.TokenExpiry,
                DeleteToken = user.DeleteToken
            };
        }

         // Validate reset token

        public async Task<bool> ValidateTokenForUser(string email, string token)
        {
            // Retrieve the user by email
            var user = await _systemUserRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return false; // User not found
            }

            // Check if the token matches and is not expired
            bool isValid = user.ResetToken == token && user.TokenExpiry > DateTime.UtcNow;
            return isValid;
        }

        // Validate email token

        public async Task<bool> ValidateEmailTokenAsync(string email, string token)
        {
            // Retrieve the user based on the provided email
            var user = await _systemUserRepository.GetUserByEmailAsync(email);
        
            // Check if user exists
            if (user == null)
            {
                return false; // Email does not exist
            }

            // Validate the token: Check if it matches the stored token and is not expired
            bool tokenIsValid = user.VerifyToken == token && user.TokenExpiry > DateTime.UtcNow;

            return tokenIsValid;
        }

        // Validate delete token
        public async Task<bool> ValidateDeleteTokenAsync(string email, string token)
        {
            // Retrieve the user based on the provided email
            var user = await _systemUserRepository.GetUserByEmailAsync(email);
        
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Validate the token: Check if it matches the stored token and is not expired
            bool tokenIsValid = user.DeleteToken == token && user.TokenExpiry > DateTime.UtcNow;

            return tokenIsValid;
        }

        public async Task<bool> ConfirmEmailAsync(string email, string token)
        {
            var user = await _systemUserRepository.GetUserByEmailAsync(email);
            
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Assume user has Token and TokenExpiry properties
            if (user.VerifyToken != token || user.TokenExpiry < DateTime.UtcNow)
            {
                throw new Exception("Invalid token or expired");
            }

            user.isVerified = true; // Set email confirmation
            user.VerifyToken = null; // Clear the reset token
            user.TokenExpiry = null; // Clear the expiry date

            await _systemUserRepository.UpdateUserAsync(user); // Update the user
            await _unitOfWork.CommitAsync(); // Commit the transaction
            return true; // Email confirmed successfully
        }

        public async Task RequestAccountDeletionAsync(SystemUserId userId)
        {
            // Verify if the user exists
            var user = await _systemUserRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            // Generate a unique token for account deletion confirmation
            var token = Guid.NewGuid().ToString();
            user.DeleteToken = token;

            // Set the token expiry time
            user.TokenExpiry = DateTime.UtcNow.AddHours(24); // Token valid for 24 hours

            // Send the account deletion confirmation email
            string deleteLink = _emailService.GenerateDeleteLink(user.Email, token);
            await _emailService.SendAccountDeletionEmailAsync(user.Email, deleteLink);

            // Commit the changes
            await _unitOfWork.CommitAsync();
        }

        // Delete user
        public async Task<SystemUserDto> DeleteFromIdAsync(SystemUserId id)
        {
            var user = await this._systemUserRepository.GetByIdAsync(id); 

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            await this._systemUserRepository.Remove(user);
            
            await this._loggingService.LogAccountDeletionAsync(id.AsString(), DateTime.UtcNow);

            await this._unitOfWork.CommitAsync();

            return new SystemUserDto
            {
                Id = user.Id.AsGuid(),
                Username = user.Username,
                Role = user.Role,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IAMId = user.IAMId,
                isVerified = user.isVerified,
                VerifyToken = user.VerifyToken,
                ResetToken = user.ResetToken,
                TokenExpiry = user.TokenExpiry,
                DeleteToken = user.DeleteToken
            };
        }
    
    }
}
