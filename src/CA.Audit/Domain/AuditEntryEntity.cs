using CA.Common.Contracts.Audit;

namespace CA.Audit.Domain
{
    public class AuditEntryEntity
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
        public string Browser { get; set; }
        public string IPAddress { get; set; }
        public string Source { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Key { get; set; }
        public string Object { get; set; }

        public static AuditEntryEntity CreateFromAuditEntry(AuditEntry entry)
        {
            return new AuditEntryEntity
            {
                Action = entry.Action,
                Browser = entry.Browser,
                CorrelationId = entry.CorrelationId,
                IPAddress = entry.IPAddress,
                TimeStamp = entry.TimeStamp,
                UserId = Guid.TryParse(entry.UserId, out Guid userId) ? userId : Guid.Empty,
                Username = entry.Username,
                Source = entry.Source,
                OldValue = entry.OldValue,
                NewValue = entry.NewValue,
                Key = entry.Key,
                Object = entry.Object
            };
        }
    }
}
