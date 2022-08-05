namespace CA.Audit.Application.Access
{
    public class AccessDto
    {
        public int Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Result { get; set; }
    }

    public class AccessDetailsDto : AccessDto
    {
        public Guid UserId { get; set; }
        public string Browser { get; set; }
        public string IPAddress { get; set; }
        public string Details { get; set; }
    }
}
