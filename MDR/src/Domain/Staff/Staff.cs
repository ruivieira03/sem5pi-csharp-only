using System;
using Hospital.Domain.Shared;
using Hospital.Domain.Users.SystemUser;

namespace Hospital.Domain.Users.staffmanagement{
    // Represents a staff member in the hospital
    public class Staff : Entity<LicenseNumber>, IAggregateRoot{
        public LicenseNumber licenseNumber { get; set; }          // License number of the staff
        public string name { get; set; }                          // Full name of the staff
        public string specialization { get; set; }                // Specialization of the staff
        public List<string> slots { get; set; }                   // List of available slots for the staff, formatted as "YYYY-MM-DD:HHhMM/HHhMM"
        public Hospital.Domain.Users.SystemUser.SystemUser systemUser { get; set; }                // SystemUser linked to the staff


          protected Staff(){ // Empty Constructor
           
        }


        // Constructor to initialize a Staff object with required properties
        public Staff(LicenseNumber licenseNumber, string name, string specialization, Hospital.Domain.Users.SystemUser.SystemUser systemUser)
        {
            this.Id = licenseNumber;                                   // Set the unique identifier for the staff
            this.licenseNumber = licenseNumber;                        // Assign the provided license number
            this.name = name;                                          // Assign the provided name
            this.specialization = specialization;                      // Assign the provided specialization
            this.slots = new List<string>();                           // Initialize the list of slots
            this.systemUser = systemUser;                              // Assign the provided SystemUser
        }

      
       
        public void AddSlots(DateTime start, DateTime end){
            slots.Add($"{start.ToString("yyyy-MM-dd")}:{start.ToString("HH'h'mm")}/{end.ToString("HH'h'mm")}");
        }
    }
}
