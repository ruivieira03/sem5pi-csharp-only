using Xunit;
using Hospital.Domain.Patients;

namespace Hospital.Tests.Domain.Unit
{
    public class PatientTests{


        
        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly(){
 
            var firstName = "John";
            var lastName = "Doe";
            var dateOfBirth = new DateTime(1990, 1, 1);
            var gender = "Male";
            var medicalRecordNumber = "202411000003";
            var email = "john.doe@example.com";
            var phoneNumber = "123456789";
            var emergencyContact = "Jane Doe - 987654321";
            var appointmentHistory = new List<string> { "Appointment1", "Appointment2" };
            var allergiesOrMedicalConditions = new List<string> { "Peanuts", "Penicillin" };

            // Act
            var patient = new Patient(firstName, lastName, dateOfBirth, gender, medicalRecordNumber, email, phoneNumber, emergencyContact, appointmentHistory, allergiesOrMedicalConditions);

            // Assert
            Assert.Equal(firstName, patient.FirstName);
            Assert.Equal(lastName, patient.LastName);
            Assert.Equal(dateOfBirth, patient.DateOfBirth);
            Assert.Equal(gender, patient.Gender);
            Assert.Equal(medicalRecordNumber, patient.MedicalRecordNumber);
            Assert.Equal(email, patient.Email);
            Assert.Equal(phoneNumber, patient.PhoneNumber);
            Assert.Equal(emergencyContact, patient.EmergencyContact);
            Assert.Equal(appointmentHistory, patient.AppointmentHistory);
            Assert.Equal(allergiesOrMedicalConditions, patient.AllergiesOrMedicalConditions);
        }



        [Fact]
        public void FullName_ShouldReturnConcatenatedFirstAndLastName(){
           
            var patient = new Patient{
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var fullName = patient.FullName;

            // Assert
            Assert.Equal("John Doe", fullName);
        }

        [Fact]
        public void UpdateProfile_ShouldUpdatePatientDetails(){
            // Arrange
            var patient = new Patient
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "old.email@example.com",
                PhoneNumber = "123456789",
                EmergencyContact = "Old Contact"
            };

            var newFirstName = "Jane";
            var newLastName = "Smith";
            var newEmail = "jane.smith@example.com";
            var newPhoneNumber = "987654321";
            var newEmergencyContact = "New Contact";

            // Act
            patient.UpdateProfile(newFirstName, newLastName, newEmail, newPhoneNumber, newEmergencyContact);

            // Assert
            Assert.Equal(newFirstName, patient.FirstName);
            Assert.Equal(newLastName, patient.LastName);
            Assert.Equal(newEmail, patient.Email);
            Assert.Equal(newPhoneNumber, patient.PhoneNumber);
            Assert.Equal(newEmergencyContact, patient.EmergencyContact);
        }
    }
}
