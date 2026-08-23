#nullable disable
using APP.Models;
using CORE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

// Generated from Custom MVC Template.

namespace MVC.Controllers
{
    public class UsersController : Controller
    {
        // Service injections:
        private readonly IService<UserRequest, UserResponse> _userService;
        private readonly IService<StatusRequest, StatusResponse> _statusService;

        /* 
            Can be uncommented and used for many to many relationships, 
            "entity" may be replaced with the related entity name in 
            the controller and views. 
        */
        private readonly IService<RoleRequest, RoleResponse> _roleService;

        public UsersController(
			IService<UserRequest, UserResponse> userService
            , IService<StatusRequest, StatusResponse> statusService

            /* 
                Can be uncommented and used for many to many relationships, 
                "entity" may be replaced with the related entity name 
                in the controller and views. 
            */
            , IService<RoleRequest, RoleResponse> roleService
        )
        {
            _userService = userService;
            _statusService = statusService;

            /* 
                Can be uncommented and used for many to many relationships, 
                "entity" may be replaced with the related entity name 
                in the controller and views. 
            */
            _roleService = roleService;
        }

        private void SetViewData()
        {
            /* 
            ViewBag and ViewData are the same collection (dictionary).
            They carry extra data other than the model from a controller 
            action to its view, or between views.
            */

            // Related items service logic to set ViewData (Id and Name
            // parameters may need to be changed in the SelectList constructor
            // according to the model):
            ViewData["StatusId"] = new SelectList(_statusService.List(), 
                "Id", "Title"); // Second parameter changed from Name to Title.

            /* 
                Can be uncommented and used for many to many relationships, 
                "entity" may be replaced with the related entity name 
                in the controller and views. 
            */
            ViewBag.RoleIds = new MultiSelectList(_roleService.List(), 
                "Id", "Name");
        }

        private void SetTempData(string message, string key = "Message")
        {
            /*
            TempData is used to carry extra data to the redirected controller action's view.
            */

            TempData[key] = message;
        }

        // GET: Users
        [Authorize] // Only authenticated users can execute this action.
        public IActionResult Index()
        {
            // Get collection service logic:
            var list = _userService.List();
            return View(list); // return response collection as model to the Index view
        }

        // GET: Users/Details/5
        [Authorize] // Only authenticated users can execute this action.
        public IActionResult Details(int id)
        {
            // Check if the user is in Admin role or trying to make
            // the operation on his/her own account.
            if (!IsOwnAccount(id) && !User.IsInRole("Admin"))
            {
                SetTempData("You are not authorized for this operation!");
                return RedirectToAction(nameof(Index));
            }

            // Get item service logic:
            var item = _userService.Single(id);
            return View(item); // return response item as model to the Details view
        }

        // GET: Users/Create
        [Authorize(Roles = "Admin")] 
        // Only authenticated users with role Admin can execute this action.
        public IActionResult Create()
        {
            SetViewData(); // set ViewData dictionary to carry extra data other than the model to the view
            return View(); // return Create view with no model
        }

        // POST: Users/Create
        [Authorize(Roles = "Admin")] 
        // Only authenticated users with role Admin can execute this action.
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(UserRequest user)
        {
            if (ModelState.IsValid) // check data annotation validation errors in the request
            {
                // Insert item service logic:
                var response = _userService.Add(user);
                if (response.IsSuccessful)
                {
                    SetTempData(response.Message); // set TempData dictionary to carry the message to the redirected action's view
                    return RedirectToAction(nameof(Details), new { id = response.Id }); // redirect to Details action with id parameter as response.Id route value
                }
                ModelState.AddModelError("", response.Message); // to display service error message in the validation summary of the view
            }
            SetViewData(); // set ViewData dictionary to carry extra data other than the model to the view
            return View(user); // return request as model to the Create view
        }

        // GET: Users/Edit/5
        [Authorize] // Only authenticated users can execute this action.
        public IActionResult Edit(int id)
        {
            // Check if the user is in Admin role or trying to make
            // the operation on his/her own account.
            if (!IsOwnAccount(id) && !User.IsInRole("Admin"))
            {
                SetTempData("You are not authorized for this operation!");
                return RedirectToAction(nameof(Index));
            }

            // Get item to edit service logic:
            var item = _userService.Edit(id);
            SetViewData(); // set ViewData dictionary to carry extra data
                           // other than the model to the view

            return View(item); // return request as model to the Edit view
        }

        // POST: Users/Edit
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize] // Only authenticated users can execute this action.
        public IActionResult Edit(UserRequest user)
        {
            // Check if the user is in Admin role or trying to make
            // the operation on his/her own account.
            if (!IsOwnAccount(user.Id) && !User.IsInRole("Admin"))
            {
                SetTempData("You are not authorized for this operation!");
                return RedirectToAction(nameof(Index));
            }

            // Because we don't get the values for the Score, StatusId and
            // RoleIds properties from the user request for users not in
            // the Admin role, and Score, StatusId and RoleIds are required,
            // we must remove the ModelState errors for these properties
            // to prevent validation failures.
            if (!User.IsInRole("Admin"))
            {
                ModelState.Remove(nameof(UserRequest.Score));
                ModelState.Remove(nameof(UserRequest.StatusId));
                ModelState.Remove(nameof(UserRequest.RoleIds));
            }

            if (ModelState.IsValid) 
            // check data annotation validation errors in the request
            {
                // Update item service logic:
                var response = _userService.Update(user);
                if (response.IsSuccessful)
                {
                    SetTempData(response.Message); 
                    // set TempData dictionary to carry the message
                    // to the redirected action's view
                    return RedirectToAction(nameof(Details), 
                        new { id = response.Id }); 
                    // redirect to Details action with id parameter as
                    // response.Id route value
                }
                ModelState.AddModelError("", response.Message); 
                // to display service error message in the
                // validation summary of the view
            }
            SetViewData(); 
            // set ViewData dictionary to carry extra data other than
            // the model to the view

            return View(user); // return request as model to the Edit view
        }

        // GET: Users/Delete/5
        [Authorize] // Only authenticated users can execute this action.
        public IActionResult Delete(int id)
        {
            // Check if the user is in Admin role or trying to make
            // the operation on his/her own account.
            if (!IsOwnAccount(id) && !User.IsInRole("Admin"))
            {
                SetTempData("You are not authorized for this operation!");
                return RedirectToAction(nameof(Index));
            }

            // Get item to delete service logic:
            var item = _userService.Single(id);
            return View(item); 
            // return response item as model to the Delete view
        }

        // POST: Users/Delete
        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        [Authorize] // Only authenticated users can execute this action.
        public IActionResult DeleteConfirmed(int id)
        {
            // Check if the user is in Admin role or trying to make
            // the operation on his/her own account.
            if (!IsOwnAccount(id) && !User.IsInRole("Admin"))
            {
                SetTempData("You are not authorized for this operation!");
                return RedirectToAction(nameof(Index));
            }

            // Delete item service logic:
            var response = _userService.Remove(id);
            SetTempData(response.Message); 
            // set TempData dictionary to carry the message
            // to the redirected action's view

            // if the user deleted his/her own account, log out the user
            if (IsOwnAccount(id))
                return RedirectToAction("Logout", "Auth");

            return RedirectToAction(nameof(Index)); 
            // redirect to the Index action
        }



        /// <summary>
        /// For regular users can only make operations on their own accounts.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <returns>bool</returns>
        bool IsOwnAccount(int id) // private is default if not written
        {
            // getting the authenticated user ID value for the claim type
            // "Id" from the user's claims and checking if it matches
            // the provided id parameter of a user
            return id.ToString() == (User.Claims.SingleOrDefault(
                claim => claim.Type == "Id")?.Value ?? string.Empty);
        }
    }
}
