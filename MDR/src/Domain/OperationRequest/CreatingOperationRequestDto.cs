using Hospital.Domain.Users.staffmanagement;

namespace Hospital.Domain.OperationRequest
{
    public class CreatingOperationRequestDto
    {
        public Guid PatientID { get; set; }
        public Guid DoctorID { get; set; }
        public string OperationTypeID { get; set; }
        public DateTime DeadlineDate { get; set; }
        public int Priority { get; set; }

        public CreatingOperationRequestDto(Guid PatientID, Guid DoctorID, string OperationTypeID, DateTime DeadlineDate, int Priority)
        {
            this.PatientID = PatientID;
            this.DoctorID = DoctorID;
            this.OperationTypeID = OperationTypeID;
            this.DeadlineDate = DeadlineDate;
            this.Priority = Priority;
        }
    }
}