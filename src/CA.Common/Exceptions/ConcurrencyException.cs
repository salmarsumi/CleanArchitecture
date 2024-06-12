namespace CA.Common.Exceptions
{
    /// <summary>
    /// The ConcurrencyException is thrown when a concurrency violation is encountered.
    /// </summary>
    public class ConcurrencyException : Exception
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="ConcurrencyException"/>.
        /// </summary>
        public ConcurrencyException() : base() { }

        /// <summary>
        /// Initialize a new instance of the <see cref="ConcurrencyException"/>.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ConcurrencyException(string message) : base(message) { }

        /// <summary>
        /// Initialize a new instance of the <see cref="ConcurrencyException"/>.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="ex">The exception that is the cause of the current exception.</param>
        public ConcurrencyException(string message, Exception ex): base(message, ex) { }
    }
}
