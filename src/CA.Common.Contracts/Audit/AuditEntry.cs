namespace CA.Common.Contracts.Audit
{
    /// <summary>
    /// Represents an audit log event used to record state change operations.
    /// </summary>
    public interface AuditEntry
    {
        /// <summary>
        /// The time the audit entry was generated.
        /// </summary>
        DateTime TimeStamp { get; }

        /// <summary>
        /// The correlation id of the current request across multiple services.
        /// </summary>
        Guid CorrelationId { get; }

        /// <summary>
        /// The user id associated with the current operation.
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// The username associated with the current operation.
        /// </summary>
        string Username { get; }

        /// <summary>
        /// The operation being performed.
        /// </summary>
        string Action { get; }

        /// <summary>
        /// The user browser agent.
        /// </summary>
        string Browser { get; }

        /// <summary>
        /// The IP address of the client.
        /// </summary>
        string IPAddress { get; }

        /// <summary>
        /// The name of entity getting a state change.
        /// </summary>
        string Object { get; }

        /// <summary>
        /// The service performing the operation.
        /// </summary>
        string Source { get; }

        /// <summary>
        /// The value of the object prior to the current state change operation.
        /// </summary>
        string OldValue { get; }

        /// <summary>
        /// The new value set by the state change operation.
        /// </summary>
        string NewValue { get; }

        /// <summary>
        /// The key identifying the current object.
        /// </summary>
        string Key { get; }
    }
}
