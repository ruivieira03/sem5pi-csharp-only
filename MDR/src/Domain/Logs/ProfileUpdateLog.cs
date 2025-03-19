namespace Hospital.Domain.Logs
{
    public class ProfileUpdateLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ChangedFields { get; set; } // Store the changed fields as a JSON string or comma-separated values
        public DateTime Timestamp { get; set; }
    }
}
