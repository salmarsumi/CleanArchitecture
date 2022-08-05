namespace CA.Common.Contracts.Audit
{
    public interface AccessEntry
    {
        Guid CorrelationId { get; }
        string UserId { get; }
        string Username { get; }
        string Action { get; }
        DateTime TimeStamp { get; }
        string Browser { get; }
        string IPAddress { get; }
        string Result { get; }
        string Details { get; }
    }
}
