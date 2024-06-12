namespace CA.Common.ResponseTypes
{
    /// <summary>
    /// Object representing an error communicated back to the caller of a service.
    /// </summary>
    public class JsonErrorResponse
    {
        /// <summary>
        /// The type of the error.
        /// </summary>
        public string ExceptionType { get; set; }

        /// <summary>
        /// The key associated with the current operation.
        /// </summary>
        public object Key { get; set; }

        /// <summary>
        /// The name of the operation or entity associated with the current operation.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Detailed development environment error message.
        /// </summary>
        public string DeveloperMessage { get; set; }

        /// <summary>
        /// Any related data.
        /// </summary>
        public object Data { get; set; }
    }
}
