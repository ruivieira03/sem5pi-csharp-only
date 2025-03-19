namespace Hospital.Domain.Logs
{
    public class AccountDeletionLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}