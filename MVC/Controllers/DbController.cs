using APP.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace MVC.Controllers
{
    // Database Controller for seeding the database with initial data
    // through the Seed action.
    public class DbController : Controller
    {
        // Action method to seed the database with initial data.
        [Route("SeedDb")] // will change the route of the action to
                          // /SeedDb instead of /Db/Seed
        public IActionResult Seed()
        {
            // Initialize the DbFactory instance.
            var dbFactory = new DbFactory();

            // Seed the database using the SeedDb method.
            dbFactory.SeedDb();

            // Return a success message as HTML content using
            // UTF-8 character set.
            return Content("<h1>Database seed successful.</h1>",
                "text/html", Encoding.UTF8);
        }
    }
}
