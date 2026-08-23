using APP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    /// <summary>
    /// Controller for managing temporary users session data 
    /// for authenticated users.
    /// Provides actions to view, add, remove and clear temporary
    /// users session data.
    /// </summary>
    [Authorize] 
    // Only authenticated users can execute this controller's actions.
    public class UserSessionController : Controller
    {
        // Constructor injection of the concrete UserSessionService instance.
        private readonly UserSessionService _userSessionService;

        public UserSessionController(UserSessionService userSessionService)
        {
            _userSessionService = userSessionService;
        }

        /// <summary>
        /// Displays the temporary users for the current authenticated user.
        /// </summary>
        /// <returns>Index view using the temporary user list model.</returns>
        public IActionResult Index() => View(_userSessionService.Get());

        /// <summary>
        /// Clears all temporary users for the current authenticated user.
        /// Sets a message and redirects to the Index action.
        /// </summary>
        /// <returns>Redirection to the Index action.</returns>
        public IActionResult Clear()
        {
            _userSessionService.Clear();
            TempData["Message"] = "All temporary users cleared.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Removes a specific user from the temporary users for the
        /// authenticated user.
        /// Sets a message and redirects to the Index action.
        /// </summary>
        /// <param name="userId">The user ID of the user to remove.</param>
        /// <returns>Redirection to the Index action.</returns>
        public IActionResult Remove(int userId)
        {
            _userSessionService.Remove(userId);
            TempData["Message"] = "Temporary user removed.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Adds a specific user to the temporary users for the authenticated user.
        /// Sets a message and redirects to the Index action
        /// of the Users controller.
        /// </summary>
        /// <param name="userId">The user ID of the user to add.</param>
        /// <returns>Redirection to the Users controller Index action.</returns>
        public IActionResult Add(int userId)
        {
            _userSessionService.Add(userId);
            TempData["Message"] = "Temporary user added.";
            return RedirectToAction("Index", "Users");
        }
    }
}
