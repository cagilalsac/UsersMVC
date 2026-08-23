using APP.Authentication.Models;
using CORE.Models;

namespace APP.Authentication.Services
{
    /// <summary>
    /// Cookie authentication service interface containing
    /// Login, Logout and Register method definitions.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user using the provided login 
        /// credentials and initiates a cookie-based sign-in.
        /// </summary>
        /// <param name="request">
        /// The LoginRequest containing the user's login 
        /// information (user name and password).
        /// </param>
        /// <returns>
        /// A CommandResponse indicating the result of the 
        /// login attempt.
        /// Returns an error response if credentials are invalid; 
        /// otherwise, returns a success response with the user's ID.
        /// Also a cookie named ".AspNetCore.Cookies" is created 
        /// in the browser to maintain the authenticated session.
        /// </returns>
        public Task<CommandResponse> Login(LoginRequest request);

        /// <summary>
        /// Signs out the currently authenticated user by removing 
        /// the ".AspNetCore.Cookies" authentication cookie.
        /// </summary>
        /// <returns>
        /// A Task representing the asynchronous sign-out operation.
        /// </returns>
        public Task Logout();

        /// <summary>
        /// Registers a new user with the default "User" role and 
        /// "Active" status.
        /// </summary>
        /// <param name="request">
        /// The RegisterRequest containing the registration data 
        /// including user name, password and confirm password.
        /// </param>
        /// <returns>
        /// A CommandResponse indicating the result of the registration.
        /// </returns>
        public CommandResponse Register(RegisterRequest request);
    }
}
