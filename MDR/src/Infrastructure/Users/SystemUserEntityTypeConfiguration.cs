using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Hospital.Domain.Users.SystemUser;
using Hospital.Services;

namespace Hospital.Infrastructure.Users
{
       internal class SystemUserEntityTypeConfiguration : IEntityTypeConfiguration<SystemUser>
       {
              private readonly IPasswordService _passwordService;

              public SystemUserEntityTypeConfiguration(IPasswordService passwordService)
              {
                     _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
              }

              public void Configure(EntityTypeBuilder<SystemUser> builder)
              {
                     // Map to the "SystemUser" table
                     builder.ToTable("SystemUser");

                     // Primary key definition
                     builder.HasKey(b => b.Id);

                     // Configure the Id property with a value converter
                     builder.Property(b => b.Id)
                            .HasConversion(
                            id => id.AsGuid(), // Convert SystemUserId to Guid
                            value => new SystemUserId(value) // Convert Guid to SystemUserId
                     );

                     // Foreign key configuration for one-to-one relationship
                     builder.HasOne(b => b.Patient)
                            .WithOne(u => u.SystemUser)
                            .HasForeignKey<SystemUser>(b => b.PatientId);

                     builder.Property(b => b.PatientId)
                            .IsRequired(false);   

                     // Property configurations
                     builder.Property(b => b.Username)
                            .IsRequired()
                            .HasMaxLength(50);

                     builder.Property(b => b.Role)
                            .IsRequired();

                     builder.Property(b => b.Password)
                            .IsRequired();

                     builder.Property(b => b.IAMId)
                            .IsRequired();

                     builder.Property(b => b.Email)
                            .IsRequired();
                     
                     builder.Property(b => b.PhoneNumber)
                            .IsRequired();

                     builder.Property(b => b.ResetToken)
                            .IsRequired(false);

                     builder.Property(b => b.VerifyToken)
                            .IsRequired(false);
                     
                     builder.Property(b => b.TokenExpiry)
                            .IsRequired(false);

                     builder.Property(b => b.isVerified)
                            .IsRequired();
                     
                     builder.Property(b => b.DeleteToken)
                            .IsRequired(false);

                     // Seed SystemUser data
                     builder.HasData(
                            new SystemUser
                            {
                            Id = new SystemUserId(Guid.NewGuid()),
                            Username = "adminUser",
                            Role = Roles.Admin,
                            Password = _passwordService.HashPassword("SEM5pi1234@"),
                            IAMId = "1",
                            Email = "ruimdv13@gmail.com",
                            PhoneNumber = "912028969",
                            isVerified = true
                     },
                            new SystemUser
                            {
                            Id = new SystemUserId(Guid.NewGuid()),
                            Username = "doctorUser",
                            Role = Roles.Doctor,
                            Password = _passwordService.HashPassword("SEM5pi1234@"),
                            IAMId = "2",
                            Email = "doctor@hospital.com",
                            PhoneNumber = "1234567891",
                            isVerified = true
                     },
                            new SystemUser
                            {
                            Id = new SystemUserId(Guid.NewGuid()),
                            Username = "staffUser",
                            Role = Roles.Staff,
                            Password = _passwordService.HashPassword("SEM5pi1234@"),
                            IAMId = "3",
                            Email = "staff@hospital.com",
                            PhoneNumber = "1234567892",
                            isVerified = true
                     }
              );
        }
    }
}
