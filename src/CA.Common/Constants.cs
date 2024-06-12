namespace CA.Common
{
    /// <summary>
    /// Common constants.
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Correlation.
        /// </summary>
        public const string CORRELATION_HEADER = "X-Correlation-Id";

        /// <summary>
        /// JWT tokens.
        /// </summary>
        public const string TOKEN_NAME = "access_token";

        /// <summary>
        /// Anti-forgery.
        /// </summary>
        public const string CSRF_HEADER = "X-CSRF";
        public const string CSRF_FORM_FIELD = "csrf-token";
        public const string CSRF_COOKIE_NAME = "__Host-CSRF-Token";
    }
}
