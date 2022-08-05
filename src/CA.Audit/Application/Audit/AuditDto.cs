namespace CA.Audit.Application.Audit
{
    public class AuditDto
    {
        public int Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Source { get; set; }
        public string Object { get; set; }
    }

    public class AuditDetailsDto : AuditDto
    {
        public Guid UserId { get; set; }
        public string Browser { get; set; }
        public string IPAddress { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Key { get; set; }
    }
}
