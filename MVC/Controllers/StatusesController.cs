#nullable disable
using APP.Models;
using CORE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

// Generated from Custom MVC Template.

namespace MVC.Controllers
{
    // Way 1:
    //[Authorize(Roles = "Admin,User")] 
    // Only authenticated users with Admin or User role can execute all
    // of the actions of this controller.
    // Way 2:
    [Authorize] 
    // Only authenticated users can execute all of the actions
    // of this controller.
    // Since we have only 2 roles Admin and User, we can use Authorize
    // to check auhenticated users without defining roles.
    public class StatusesController : Controller
    {
        // Service injections:
        private readonly IService<StatusRequest, StatusResponse> _statusService;

        /* Can be uncommented and used for many to many relationships, "entity" may be replaced with the related entity name in the controller and views. */
        //private readonly IService<EntityRequest, EntityResponse> _EntityService;

        public StatusesController(
			IService<StatusRequest, StatusResponse> statusService

            /* Can be uncommented and used for many to many relationships, "entity" may be replaced with the related entity name in the controller and views. */
            //, IService<EntityRequest, EntityResponse> EntityService
        )
        {
            _statusService = statusService;

            /* Can be uncommented and used for many to many relationships, "entity" may be replaced with the related entity name in the controller and views. */
            //_EntityService = EntityService;
        }

        private void SetViewData()
        {
            /* 
            ViewBag and ViewData are the same collection (dictionary).
            They carry extra data other than the model from a controller action to its view, or between views.
            */

            // Related items service logic to set ViewData (Id and Name parameters may need to be changed in the SelectList constructor according to the model):
            
            /* Can be uncommented and used for many to many relationships, "entity" may be replaced with the related entity name in the controller and views. */
            //ViewBag.EntityIds = new MultiSelectList(_EntityService.List(), "Id", "Name");
        }

        private void SetTempData(string message, string key = "Message")
        {
            /*
            TempData is used to carry extra data to the redirected controller action's view.
            */

            TempData[key] = message;
        }

        // GET: Statuses
        [AllowAnonymous] 
        // Can be used to allow authenticated and unauthenticated users (everyone)
        // to execute this action.
        // Overrides the Authorize defined for the controller.
        public IActionResult Index()
        {
            // Get collection service logic:
            var list = _statusService.List();
            return View(list); // return response collection as model to the Index view
        }

        // GET: Statuses/Details/5
        // Only authenticated users (since Authorize is defined at the controller
        // level) can execute this action.
        public IActionResult Details(int id)
        {
            // Get item service logic:
            var item = _statusService.Single(id);
            return View(item); // return response item as model to the Details view
        }

        // GET: Statuses/Create
        [Authorize(Roles = "Admin")] 
        // Only authenticated users with role Admin can execute this action.
        // Overrides the Authorize defined for the controller.
        public IActionResult Create()
        {
            SetViewData(); // set ViewData dictionary to carry extra data other than the model to the view
            return View(); // return Create view with no model
        }

        // POST: Statuses/Create
        [Authorize(Roles = "Admin")]
        // Only authenticated users with role Admin can execute this action.
        // Overrides the Authorize defined for the controller.
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(StatusRequest status)
        {
            if (ModelState.IsValid) // check data annotation validation errors in the request
            {
                // Insert item service logic:
                var response = _statusService.Add(status);
                if (response.IsSuccessful)
                {
                    SetTempData(response.Message); // set TempData dictionary to carry the message to the redirected action's view
                    return RedirectToAction(nameof(Details), new { id = response.Id }); // redirect to Details action with id parameter as response.Id route value
                }
                ModelState.AddModelError("", response.Message); // to display service error message in the validation summary of the view
            }
            SetViewData(); // set ViewData dictionary to carry extra data other than the model to the view
            return View(status); // return request as model to the Create view
        }

        // GET: Statuses/Edit/5
        [Authorize(Roles = "Admin")]
        // Only authenticated users with role Admin can execute this action.
        // Overrides the Authorize defined for the controller.
        public IActionResult Edit(int id)
        {
            // Get item to edit service logic:
            var item = _statusService.Edit(id);
            SetViewData(); // set ViewData dictionary to carry extra data other than the model to the view
            return View(item); // return request as model to the Edit view
        }

        // POST: Statuses/Edit
        [Authorize(Roles = "Admin")]
        // Only authenticated users with role Admin can execute this action.
        // Overrides the Authorize defined for the controller.
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(StatusRequest status)
        {
            if (ModelState.IsValid) // check data annotation validation errors in the request
            {
                // Update item service logic:
                var response = _statusService.Update(status);
                if (response.IsSuccessful)
                {
                    SetTempData(response.Message); // set TempData dictionary to carry the message to the redirected action's view
                    return RedirectToAction(nameof(Details), new { id = response.Id }); // redirect to Details action with id parameter as response.Id route value
                }
                ModelState.AddModelError("", response.Message); // to display service error message in the validation summary of the view
            }
            SetViewData(); // set ViewData dictionary to carry extra data other than the model to the view
            return View(status); // return request as model to the Edit view
        }

        // GET: Statuses/Delete/5
        [Authorize(Roles = "Admin")]
        // Only authenticated users with role Admin can execute this action.
        // Overrides the Authorize defined for the controller.
        public IActionResult Delete(int id)
        {
            // Get item to delete service logic:
            var item = _statusService.Single(id);
            return View(item); // return response item as model to the Delete view
        }

        // POST: Statuses/Delete
        [Authorize(Roles = "Admin")]
        // Only authenticated users with role Admin can execute this action.
        // Overrides the Authorize defined for the controller.
        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            // Delete item service logic:
            var response = _statusService.Remove(id);
            SetTempData(response.Message); // set TempData dictionary to carry the message to the redirected action's view
            return RedirectToAction(nameof(Index)); // redirect to the Index action
        }
    }
}
