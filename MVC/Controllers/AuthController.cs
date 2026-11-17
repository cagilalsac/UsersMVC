using APP.Authentication.Models;
using APP.Authentication.Services;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    /// <summary>
    /// Controller for authentication operations such as 
    /// Login, Logout and Register.
    /// </summary>
    public class AuthController : Controller
    {
        // AuthService constructor injection.
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }



        /// <summary>
        /// Displays the login view for user authentication.
        /// Route changed from GET: /Auth/Login to 
        /// GET: /Login for easier access by the Route
        /// attribute below.
        /// </summary>
        [Route("~/[action]")]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Handles user login form submission.
        /// Route changed from POST: /Auth/Login to
        /// POST: /Login for easier access by the Route
        /// attribute below.
        /// </summary>
        /// <param name="request">The login request containing user 
        /// credentials (UserName and Password).</param>
        /// <returns>
        /// Redirects to Home/Index on successful login; otherwise, 
        /// redisplays the login view with validation errors or 
        /// authentication failure message.
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        [Route("~/[action]")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (ModelState.IsValid) 
            // Validate input model (checks required fields and
            // string lengths through data annotations).
            {
                var response = await _authService.LoginAsync(request); 
                // Attempt to authenticate user.
                if (response.IsSuccessful)
                    return RedirectToAction("Index", "Home"); 
                    // On success, redirect to home page.
                ModelState.AddModelError("", response.Message); 
                // On failure, show error message in the validation summary
                // of the view.
            }
            return View(); 
            // Redisplay login view with validation errors or
            // authentication failure message.
        }

        /// <summary>
        /// Logs out the current user and redirects to Login.
        /// Route changed from GET: /Auth/Logout to 
        /// GET: /Logout for easier access by the Route 
        /// attribute below.
        /// </summary>
        /// <remarks>This method terminates the user's 
        /// authentication cookie.</remarks>
        /// <returns>Redirects to Login.</returns>
        [Route("~/[action]")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync(); 
            // Sign out the user.
            return RedirectToAction(nameof(Login)); 
            // Redirect to the Login view through the Login action.
        }

        /// <summary>
        /// Displays the register view for user registration.
        /// Route changed from GET: /Auth/Register to 
        /// GET: /Register for easier access by the Route
        /// attribute below.
        /// </summary>
        [Route("~/[action]")]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Handles user register form submission.
        /// Route changed from POST: /Auth/Register to
        /// POST: /Register for easier access by the Route
        /// attribute below.
        /// </summary>
        /// <param name="request">The register request containing 
        /// UserName, Password and ConfirmPassword.</param>
        /// <returns>
        /// Redirects to Login on successful registration; otherwise, 
        /// redisplays the register view with validation errors 
        /// or registration failure message.
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken, Route("~/[action]")]
        public IActionResult Register(RegisterRequest request)
        {
            if (ModelState.IsValid) 
            // Validate input model through data annotations.
            {
                var response = _authService.Register(request); 
                // Attempt to register user.
                if (response.IsSuccessful)
                    return RedirectToAction(nameof(Login)); 
                // On success, redirect to Login view through the
                // Login action.
                ModelState.AddModelError("", response.Message); 
                // On failure, show error message in the validation summary
                // of the view.
            }
            return View(request); 
            // Redisplay register view with the model and validation errors
            // or registration failure message.
        }
    }
}
