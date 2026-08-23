using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CORE.Authentication.Services
{
    /// <summary>
    /// Provides cookie-based authentication services for 
    /// signing in and signing out users.
    /// </summary>
    public class CookieAuthService : ICookieAuthService
    {
        // Field injected by the constructor to be used in
        // SignIn and SignOut methods below.
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the CookieAuthService class.
        /// </summary>
        /// <param name="httpContextAccessor">Provides access 
        /// to the current HTTP context. Injection must be 
        /// managed in the IoC Container of Program.cs.</param>
        public CookieAuthService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Readonly property to get the authenticated user's ID value.
        public int UserId => Convert.ToInt32(
            _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(
                claim => claim.Type == "Id").Value);

        /// <summary>
        /// Signs in a user by creating an authentication cookie 
        /// with the specified claims and properties.
        /// </summary>
        /// <param name="userId">The unique identifier of the 
        /// user.</param>
        /// <param name="userName">The user name of the user.
        /// </param>
        /// <param name="userRoleNames">A collection of role 
        /// names assigned to the user.</param>
        /// <param name="expiration">Optional expiration date 
        /// and time for the authentication cookie. 
        /// If not specified, a default value (null) is used.
        /// </param>
        /// <param name="isPersistent">Indicates whether the 
        /// authentication cookie should persist across browser 
        /// sessions.</param>
        /// <returns>A task representing the asynchronous 
        /// sign-in operation.</returns>
        public async Task SignIn(int userId, string userName, 
            IEnumerable<string> userRoleNames, 
            DateTime? expiration = default, 
            bool isPersistent = true)
        {
            // Create claims for user ID and username,
            // then add claims for each user role.
            var claims = new List<Claim>()
            {
                new Claim("Id", userId.ToString()), 
                // custom claim with key Id and value user ID
                new Claim(ClaimTypes.Name, userName)
                // claim with key Name and value user name
            };
            foreach (var userRoleName in userRoleNames)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRoleName));
                // claim with key Role and value user's role name
            }

            // Create a ClaimsIdentity with the generated claims
            // and specify the authentication scheme ("Cookies").
            var identity = new ClaimsIdentity(claims, 
                CookieAuthenticationDefaults.AuthenticationScheme);

            // Create a ClaimsPrincipal from the identity.
            var principal = new ClaimsPrincipal(identity);

            // Set authentication properties,
            // including persistence and expiration.
            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                ExpiresUtc = expiration.HasValue
                    ? DateTime.SpecifyKind(expiration.Value, DateTimeKind.Utc) 
                    : null
            };

            // Sign in the user by issuing the authentication cookie.
            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, principal, 
                authenticationProperties);
        }

        /// <summary>
        /// Signs out the current user by removing the authentication cookie.
        /// </summary>
        /// <returns>A task representing the asynchronous sign-out operation.
        /// </returns>
        public async Task SignOut()
        {
            // Sign out the user by removing the authentication cookie
            // from the current HTTP context.
            await _httpContextAccessor.HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
