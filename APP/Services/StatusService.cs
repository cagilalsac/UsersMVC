using APP.Domain;
using APP.Models;
using CORE.Models;
using CORE.Services;
using Microsoft.EntityFrameworkCore;

namespace APP.Services
{
    // StatusService inherits the DbService class for Status entity type,
    // therefore Status entity database operations of the DbService
    // class can be used in this class.
    // StatusService also implements the IService interface for
    // StatusRequest and StatusResponse types, therefore IService method
    // definitions can be implemented for types StatusRequest and
    // StatusResponse in this class, which will be used by
    // the controller actions.
    public class StatusService : DbService<Status>,
        IService<StatusRequest, StatusResponse>
    {
        // Constructor that passes the injected DbContext instance
        // to the DbService base class constructor.
        public StatusService(DbContext db) : base(db)
        {
        }

        // Overrides the base virtual DbQuery method to include
        // User entities data to the query to be used in the
        // Query and Remove methods and orders the query by Status
        // entity Title. The overridden DbQuery method
        // can be used in any method below by invoking
        // the DbQuery method.
        protected override IQueryable<Status> DbQuery()
        {
            return base.DbQuery() // "select * from Statuses"
                                  // entity query.
                .Include(s => s.Users) // Includes the relational
                                       // User entities data to the query
                                       // by using left outer join.
                .OrderBy(s => s.Title); // Orders the query by Title
                                        // property of the Status entity
                                        // in ascending order.
                                        // OrderByDescending can be used
                                        // for descending order.
            // s: Status entity delegate.
        }

        // Gets the updated query from the DbQuery method then projects
        // the Status entities to status responses and returns the
        // status response query.
        // Example query: "select Title, UserNamesR, UserCountR from Statuses
        // left outer join Users on Statuses.Id = Users.StatusId order by
        // Title". where Title, UserNamesR and UserCountR are the
        // StatusResponse properties.
        // The returned query can be executed by invoking
        // ToList, SingleOrDefault, FirstOrDefault, Any, etc. LINQ
        // (Language Integrated Query) methods and then the returned result
        // can be used.
        // This projected query by the Select LINQ method will be used by
        // the List and Single methods of the IService interface.
        public IQueryable<StatusResponse> Query()
        {
            return DbQuery()
                .Select(s => new StatusResponse
                {
                    // Map each Status entity property to
                    // StatusResponse property.
                    Id = s.Id,
                    Title = s.Title,
                    // Way 1: Map primitive type data for basic information.
                    UserNamesR = string.Join(", ", s.Users.Select(u => u.UserName)),
                    // string type's Join method gets a seperator as the first
                    // parameter and gets a collection of strings as the second
                    // parameter then concatenates each string item in collection
                    // with the seperator and returns a string.
                    UserCountR = s.Users.Count,
                    // Count property of the List type returns the item count 
                    // of the collection.
                    // Way 2: Map complex type object for more information.
                    // Either Way 1, Way 2 or both can be used.
                    UsersR = s.Users.Select(u => new UserResponse
                    {
                        // Map each User entity property to
                        // StatusResponse's UserResponse property.
                        Id = u.Id,
                        UserName = u.UserName,
                        PasswordR = new string('*', u.Password.Length),
                        // Let's hide the password with *.
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        FullNameR = u.FirstName + " " + u.LastName,
                        // Concatenate the first name and last name
                        // with a white space for the full name.
                        RegistrationDate = u.RegistrationDate,
                        BirthDate = u.BirthDate,
                        Gender = u.Gender,
                        IsOnline = u.IsOnline,
                        Score = u.Score,
                        StatusId = u.StatusId
                    }).ToList()
                });
            // s: Status entity delegate, u: User entity delegate.
        }

        // Gets the Status entity by ID then returns a mapped
        // StatusRequest instance from the found Status entity.
        public StatusRequest Edit(int id)
        {
            var statusEntity = DbSingle(id);
            // Example SQL: "select * from Statuses where Id = 5".

            // statusEntity may be null since the OrDefault version
            // of the Single method is used. If Single method
            // was used, an exception would be thrown.
            if (statusEntity is null) // if (statusEntity == null) can
                                      // also be written.
                return null;

            // Map each Status entity property to StatusRequest property
            // and return the StatusRequest instance.
            return new StatusRequest
            {
                Id = statusEntity.Id,
                Title = statusEntity.Title
            };
        }

        // Checks whether the status with the same trimmed and
        // case sensitive title exists in the Statuses database table
        // first. If it doesn't exist, inserts the Status entity
        // mapped from the StatusRequest instance to the Statuses
        // database table.
        public CommandResponse Add(StatusRequest request)
        {
            if (DbQuery()
                .Any(s => s.Title == request.Title.Trim()))
                return Error("Status with the same title exists!");
            // s: Status entity delegate.
            // Trim method removes white space characters in the
            // beginning and at the end.

            // Create a new Status entity instance from the
            // StatusRequest instance then insert the entity in the
            // Statuses database table (second save default parameter
            // of the DbAdd method is true therefore changes of the
            // DbSets are commited to the related database tables).
            var statusEntity = new Status
            {
                Title = request.Title.Trim()
            };
            DbAdd(statusEntity);

            // Entity's Id value will be updated by the database
            // after the insert operation.
            return Success(statusEntity.Id, "Status created successfully.");
        }

        // Checks whether the status with the same trimmed and
        // case sensitive title other than the status request record
        // exists in the Statuses database table first.
        // If the entity doesn't exist, gets the Status entity by ID
        // from the table, then updates the Status entity properties
        // from status request properties and finally updates the
        // Status entity in the Statuses database table.
        public CommandResponse Update(StatusRequest request)
        {
            if (DbQuery()
                .Any(s => s.Id != request.Id &&
                    s.Title == request.Title.Trim()))
                return Error("Status with the same title exists!");

            // Get the Status entity by ID from the Statuses database
            // table and check whether it is null or not.
            var statusEntity = DbSingle(request.Id);
            if (statusEntity is null)
                return Error("Status not found!");

            // Update Status entity properties from status request properties.
            // Then update the entity in the Statuses database table (second save
            // default parameter of the DbUpdate method is true
            // therefore changes of the DbSets are commited to the related
            // database tables).
            statusEntity.Title = request.Title.Trim();
            DbUpdate(statusEntity);

            return Success(statusEntity.Id, "Status updated successfully.");
        }

        // Gets the Status entity by ID from the Statuses database table.
        // If the entity exists, checks if the Status entity has
        // relational User entities. If not, deletes the entity from the
        // Statuses database table.
        public CommandResponse Remove(int id)
        {
            // Get the Status entity by ID from the Statuses database table
            // and check whether it is null or not.
            var statusEntity = DbSingle(id);
            if (statusEntity is null)
                return Error("Status not found!");

            // Check if the Status entity has relational User entities.
            if (statusEntity.Users.Any()) // if (statusEntity.Users.Count > 0)
                                          // can also be written.
                return Error("Status has relational users!");

            // Delete the Status entity from the Statuses database table
            // (second save default parameter of the DbRemove method is
            // true therefore changes of the DbSets are commited to
            // the related database tables).
            DbRemove(statusEntity);

            return Success(statusEntity.Id, "Status deleted successfully.");
        }
    }
}
