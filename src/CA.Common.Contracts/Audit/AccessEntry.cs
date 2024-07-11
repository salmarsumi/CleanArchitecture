namespace CA.Common.Contracts.Audit
{
    /// <summary>
    /// Represents an access log event used to record login, logout, and failed permission operations.
    /// </summary>
    public interface AccessEntry
    {
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
        /// The time the access entry was generated.
        /// </summary>
        DateTime TimeStamp { get; }
        
        /// <summary>
        /// The user browser agent.
        /// </summary>
        string Browser { get; }
        
        /// <summary>
        /// The IP address of the client.
        /// </summary>
        string IPAddress { get; }
        
        /// <summary>
        /// The result of the operation.
        /// </summary>
        string Result { get; }

        /// <summary>
        /// The detailed result of the operation.
        /// </summary>
        string Details { get; }
    }
}
