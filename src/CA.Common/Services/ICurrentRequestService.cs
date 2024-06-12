using System.Security.Claims;

namespace CA.Common.Services
{
    /// <summary>
    /// Current request information operations.
    /// </summary>
    public interface ICurrentRequestService
    {
        /// <summary>
        /// Get the Id of the current user.
        /// </summary>
        /// <returns>The user Id</returns>
        string GetUserId();
        
        /// <summary>
        /// Get the name of the current user.
        /// </summary>
        /// <returns>The user name.</returns>
        string GetUsername();
        
        /// <summary>
        /// Check if the request is authenticated.
        /// </summary>
        /// <returns>A flag indicating the authenication state.</returns>
        bool IsAuthenticated();

        /// <summary>
        /// Get the instance of the current user.
        /// </summary>
        /// <returns>The user instance as <see cref="ClaimsPrincipal"/>.</returns>
        ClaimsPrincipal GetUser();

        /// <summary>
        /// Get the correlation id of the current request.
        /// </summary>
        /// <returns>The correlation id.</returns>
        string GetCorrelationId();

        /// <summary>
        /// The IP address of the current client.
        /// </summary>
        /// <returns>The client IP address.</returns>
        string GetClientIPAddress();

        /// <summary>
        /// The client browser of the current request.
        /// </summary>
        /// <returns>The client browser.</returns>
        string GetClientBrowser();
    }
}
