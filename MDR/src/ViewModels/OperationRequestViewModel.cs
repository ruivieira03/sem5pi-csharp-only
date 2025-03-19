using System;
using System.ComponentModel.DataAnnotations;
using Hospital.Domain.OperationRequest;
using Hospital.Domain.Users.staffmanagement;

namespace Hospital.ViewModels
{
    public class OperationRequestViewModel
    {
        [Required(ErrorMessage = "Patient ID is required")]
        public Guid PatientID { get; set; }

        [Required(ErrorMessage = "Doctor ID is required")]
        public Guid DoctorID { get; set; }

        [Required(ErrorMessage = "Operation Type ID is required")]
        public string OperationTypeID { get; set; }
        
        [Required(ErrorMessage = "Deadline Date is required")]
        public DateTime DeadlineDate { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        public int Priority { get; set; }
    }
}