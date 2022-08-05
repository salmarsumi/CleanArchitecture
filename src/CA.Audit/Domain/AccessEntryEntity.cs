using CA.Common.Contracts.Audit;

namespace CA.Audit.Domain
{
    public class AccessEntryEntity
    {
        public int Id { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Browser { get; set; }
        public string IPAddress { get; set; }
        public string Result { get; set; }
        public string Details { get; set; }

        public static AccessEntryEntity CreateFromAccessEntry(AccessEntry entry)
        {
            return new AccessEntryEntity
            {
                Action = entry.Action,
                Browser = entry.Browser,
                CorrelationId = entry.CorrelationId,
                Details = entry.Details,
                IPAddress = entry.IPAddress,
                Result = entry.Result,
                TimeStamp = entry.TimeStamp,
                UserId = Guid.TryParse(entry.UserId, out Guid userId) ? userId : Guid.Empty,
                Username = entry.Username
            };
        }
    }

}
