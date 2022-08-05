namespace CA.Common.Contracts.Audit
{
    public interface AuditEntry
    {
        DateTime TimeStamp { get; }
        Guid CorrelationId { get; }
        string UserId { get; }
        string Username { get; }
        string Action { get; }
        string Browser { get; }
        string IPAddress { get; }
        string Object { get; }
        string Source { get; }
        string OldValue { get; }
        string NewValue { get; }
        string Key { get; }
    }
}
