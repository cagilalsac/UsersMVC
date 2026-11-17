using APP.Authentication.Models;
using APP.Domain;
using CORE.Authentication.Services;
using CORE.Models;
using CORE.Services;
using Microsoft.EntityFrameworkCore;

namespace APP.Authentication.Services
{
    /// <summary>
    /// Cookie authentication service concrete class 
    /// inheriting from the base abstract DbService class
    /// for User entity query, insert and update database operations 
    /// and implementing the IAuthService method definitions 
    /// for log in, log out and register operations.
    /// </summary>
    public class AuthService : DbService<User>, IAuthService
    {
        /// <summary>
        /// Service interface for cookie authentication 
        /// including sign in and sign out methods.
        /// The injected instance in the constructor is assigned 
        /// to this field to be used in Login and Logout
        /// methods below.
        /// </summary>
        private readonly ICookieAuthService _cookieAuthService;

        /// <summary>
        /// Initializes a new instance of the AuthService class.
        /// Uses dependency injection to receive the database context 
        /// and cookie authentication service.
        /// </summary>
        /// <param name="db">
        /// The DbContext instance used for database operations, 
        /// which the injection is managed in the IoC Container 
        /// of Program.cs.
        /// </param>
        /// <param name="cookieAuthService">
        /// The ICookieAuthService instance used for cookie authentication, 
        /// which the injection is managed in the IoC Container 
        /// of Program.cs.
        /// </param>
        public AuthService(DbContext db, 
            ICookieAuthService cookieAuthService) : base(db)
        {
            _cookieAuthService = cookieAuthService;
        }

        // Overridden base DbQuery method to include Status, UserRole
        // and Role entities to the query.
        protected override IQueryable<User> DbQuery()
        {
            return base.DbQuery()
                .Include(u => u.Status)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role);
            // u: User entity delegate, ur: UserRole entity delegate.
        }

        /// <summary>
        /// Authenticates a user using the provided login 
        /// credentials and initiates a cookie-based sign-in.
        /// </summary>
        /// <param name="request">
        /// The LoginRequest containing the user's login 
        /// information (user name and password).
        /// </param>
        /// <returns>
        /// A CommandResponse indicating the result of the login attempt.
        /// Returns an error response if credentials are invalid; 
        /// otherwise, returns a success response with the user's ID.
        /// Also a cookie named ".AspNetCore.Cookies" is created 
        /// in the browser to maintain the authenticated session.
        /// </returns>
        public async Task<CommandResponse> LoginAsync(LoginRequest request)
        {
            /*
            Synchronous methods execute tasks one after another. 
            Each operation must complete before the next one starts. 
            The calling thread waits (or "blocks") until the method finishes. 
            
            Asynchronous methods allow tasks to run in the background. 
            The calling thread does not wait for the operation to finish and 
            can continue executing other code. In C#, asynchronous methods 
            often use the async and await keywords, enabling non-blocking 
            operations (such as I/O or database calls) and improving 
            application responsiveness.
            */
            // Attempt to find an active user matching the provided
            // user name and password.
            var userEntity = DbSingle(u => u.UserName == request.UserName
                    && u.Password == request.Password
                    && u.StatusId == 1);
                    // "Active" status ID value is 1 in the Statuses table.
                    // Instead of u.StatusId == 1, u.Status.Title == "Active"
                    // may also be written. 
            // u: User entity delegate, ur: UserRole entity delegate.

            // If no matching user entity is found, return an error response.
            if (userEntity is null)
                return Error("Invalid user name or password!");

            // Update the User entity's online status
            // (Optional if the User entity has an online status property).
            userEntity.IsOnline = true;
            DbUpdate(userEntity);

            // Sign in the user using cookie authentication,
            // passing user ID, user name, and role names
            // as parameters.
            await _cookieAuthService.SignInAsync(
                userEntity.Id,
                userEntity.UserName,
                userEntity.UserRoles.Select(ur => ur.Role.Name));

            // Return a success response with the user's ID.
            return Success(userEntity.Id, "User logged in successfully.");
        }

        /// <summary>
        /// Signs out the currently authenticated user by removing 
        /// the ".AspNetCore.Cookies" authentication cookie.
        /// </summary>
        /// <returns>
        /// A Task representing the asynchronous sign-out operation.
        /// </returns>
        public async Task LogoutAsync()
        {
            // Find the User entity by authenticated user's ID
            // to update the online status.
            // (Optional if the User entity has an online status property).
            var userEntity = DbSingle(_cookieAuthService.UserId);
            // If User entity is found, update the online status.
            if (userEntity is not null)
            {
                userEntity.IsOnline = false;
                DbUpdate(userEntity);
            }

            // Perform sign out using the cookie authentication service.
            await _cookieAuthService.SignOutAsync();
        }

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
        public CommandResponse Register(RegisterRequest request)
        {
            // Check if a user with the same user name exists.
            if (DbQuery().Any(u => u.UserName == request.UserName.Trim()))
                return Error("User with the same user name exists!");

            // Insert a new User entity with request's user name and password,
            // status "Active" and role "User".
            var userEntity = new User
            {
                UserName = request.UserName.Trim(),
                Password = request.Password.Trim(),
                StatusId = 1, 
                // "Active" status ID value is 1 in the Statuses table.

                // Way 1:
                //RoleIds = new List<int> { 2 }
                // Way 2:
                RoleIds = [2] 
                // "User" role ID value is 2 in the Roles table.
            };
            DbAdd(userEntity);
            return Success(userEntity.Id, "User registered successfully.");
        }
    }
}
