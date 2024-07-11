namespace CA.MediatR
{
    /// <summary>
    /// Implemented by the request to indicate the need to redact sensitive information from the request before logging it.
    /// </summary>
    /// <typeparam name="T">The type of request being marked as security sensitive.</typeparam>
    public interface ISecuritySensitive<T>
    {
        /// <summary>
        /// Returns a redacted copy of the current request.
        /// </summary>
        /// <returns>An instance of T without any sensitive information.</returns>
        T GetSafeCopy();
    }
}
