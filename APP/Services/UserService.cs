using APP.Domain;
using APP.Models;
using CORE.Models;
using CORE.Services;
using Microsoft.EntityFrameworkCore;

namespace APP.Services
{
    // UserService inherits the DbService class for User entity type,
    // therefore User entity database operations of the DbService
    // class can be used in this class.
    // UserService also implements the IService interface for
    // UserRequest and UserResponse types, therefore IService method
    // definitions can be implemented for types UserRequest and
    // UserResponse in this class, which will be used by
    // the controller actions.
    public class UserService : DbService<User>,
        IService<UserRequest, UserResponse>
    {
        // Constructor that passes the injected DbContext instance
        // to the DbService base class constructor.
        public UserService(DbContext db) : base(db)
        {
        }

        // Overrides the base virtual DbQuery method to include
        // Status, UserRole and Role entities data to the query
        // to be used in the Query, Edit, Update and Remove
        // methods and orders the query by User entity Score
        // property descending first, then the ordered records
        // are ordered descending by User entity RegistrationDate
        // property. Finally the ordered records are ordered
        // ascending by User entity UserName property.
        // The overridden DbQuery method can be used in any method
        // below by invoking the DbQuery method.
        protected override IQueryable<User> DbQuery()
        {
            // Relationships:
            // User <-> Status.
            // User <-> UserRoles <-> Roles.
            return base.DbQuery()
                .Include(u => u.Status) // Include Status entities
                                        // from User entities.
                .Include(u => u.UserRoles) // Include UserRole entities
                                           // from User entities.
                .ThenInclude(ur => ur.Role) // Then include Role entities
                                            // from UserRole entities.
                .OrderByDescending(u => u.Score)
                .ThenByDescending(u => u.RegistrationDate)
                .ThenBy(u => u.UserName);
                // Only one OrderBy method must be called.
                // The other called ordering methods must be ThenBy.
            // u: User entity delegate, ur: UserRole entity delegate.
        }

        // Gets the updated query from the DbQuery method then projects
        // the User entities to user responses and returns the
        // user response query.
        // The returned query can be executed by invoking
        // ToList, SingleOrDefault, FirstOrDefault, Any, etc. LINQ
        // (Language Integrated Query) methods and then the returned result
        // can be used.
        // This projected query by the Select LINQ method will be used by
        // the List and Single methods of the IService interface.
        public IQueryable<UserResponse> Query()
        {
            return DbQuery().Select(u => new UserResponse
            {
                // Map each User entity property to
                // UserResponse property.
                Id = u.Id,
                UserName = u.UserName,
                Password = u.Password,
                FirstName = u.FirstName,
                LastName = u.LastName,
                RegistrationDate = u.RegistrationDate,
                BirthDate = u.BirthDate,
                IsOnline = u.IsOnline,
                Gender = u.Gender,
                Score = u.Score,
                StatusId = u.StatusId,
                // Way 1: Map primitive type data for basic information.
                StatusTitleR = u.Status.Title,
                // Way 2: Map complex type object for more information.
                // Either Way 1, Way 2 or both can be used.
                StatusR = new StatusResponse
                {
                    // Map each Status entity property to
                    // UserResponse's StatusResponse property.
                    Id = u.Status.Id,
                    Title = u.Status.Title
                },
                // Way 1: Map primitive type data for basic information.
                RoleNamesR = string.Join("<br>", u.UserRoles
                    .OrderBy(ur => ur.Role.Name)
                    .Select(ur => ur.Role.Name)),
                // Way 2: Map complex type object for more information.
                // Either Way 1, Way 2 or both can be used.
                RolesR = u.UserRoles.OrderBy(ur => ur.Role.Name)
                    .Select(ur => new RoleResponse
                {
                    // Map each Role entity property to
                    // UserResponse's RoleResponse property through UserRoles.
                    Id = ur.Role.Id,
                    Name = ur.Role.Name
                }).ToList(),
                // Hidden password with * character.
                PasswordR = new string('*', u.Password.Length),
                // Concatenated first name and list name with white space character.
                FullNameR = u.FirstName + " " + u.LastName,
                // Formatted date and time in month/day/year hour:minute:second
                // format. No need to use the CultureInfo instance for the second
                // parameter of the ToString method since CultureInfo has been
                // assigned in the base abstract Service class by the Culture
                // property assignment. If Culture property of the base abstract
                // Service class is changed within this class constructor,
                // the changed value will be used by the CultureInfo instance and
                // then ToString method.
                RegistrationDateR = u.RegistrationDate
                    .ToString("MM/dd/yyyy HH:mm.ss"),
                // Ternary operator:
                BirthDateR = u.BirthDate.HasValue ? // if (u.BirthDate != null).
                    u.BirthDate.Value.ToShortDateString() : // Assign u.BirthDate's
                                                            // value since u.BirthDate
                                                            // is nullable.
                    string.Empty, // "" string.
                // ToShortDateString returns the date in month/day/year format.
                IsOnlineR = u.IsOnline ? "Yes" : "No",
                GenderR = u.Gender.ToString(), // Assigns "Man" or "Woman".
                ScoreR = u.Score.ToString("N1") // N: Number format.
                                                // 1: 1 decimal.
                                                // C can also be used for currency.
                                                // No need to define the second
                                                // CultureInfo parameter since Culture
                                                // property has been assigned in the
                                                // base abstract Service class.
            });
            // u: User entity delegate, ur: UserRole entity delegate.
        }

        // Gets the User entity by ID then returns a mapped
        // UserRequest instance from the found User entity.
        public UserRequest Edit(int id)
        {
            var userEntity = DbSingle(id);
            // Example SQL: "select * from Users where Id = 7".

            // userEntity may be null since the OrDefault version
            // of the Single method is used. If Single method
            // was used, an exception would be thrown.
            if (userEntity is null) // if (userEntity == null) can
                                    // also be written.
                return null;

            // Map each User entity property to UserRequest property
            // and return the UserRequest instance.
            return new UserRequest
            {
                Id = userEntity.Id,
                UserName = userEntity.UserName,
                Password = userEntity.Password,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                BirthDate = userEntity.BirthDate,
                Gender = userEntity.Gender,
                Score = userEntity.Score,
                StatusId = userEntity.StatusId,
                RoleIds = userEntity.RoleIds
            };
        }

        // Checks whether the user with the same trimmed and
        // case sensitive user name exists in the Users database table
        // first. If it doesn't exist, inserts the User entity
        // mapped from the UserRequest instance to the Users
        // database table.
        public CommandResponse Add(UserRequest request)
        {
            if (DbQuery().Any(u => 
                u.UserName == request.UserName.Trim()
                //&& u.BirthDate == request.BirthDate
                // One or many conditions can be used by && (and), || (or)
                // and ! (not) operators.
            ))
                return Error("User with the same user name exists!");
            // u: User entity delegate.
            // Trim method removes white space characters in the
            // beginning and at the end.

            // Create a new User entity instance from the
            // UserRequest instance then insert the entity in the
            // Users database table (second save default parameter
            // of the DbAdd method is true therefore changes of the
            // DbSets are commited to the related database tables).
            var userEntity = new User
            {
                UserName = request.UserName.Trim(),
                Password = request.Password.Trim(),
                FirstName = request.FirstName?.Trim(),
                // Since request.FirstName may be null, ? is used after
                // meaning that if request.FirstName is null, assign
                // null to the User entity's FirstName property else
                // assign the trimmed value of request.FirstName.
                LastName = request.LastName?.Trim(),
                // Since request.LastName may be null, ? is used after
                // meaning that if request.LastName is null, assign
                // null to the User entity's LastName property else
                // assign the trimmed value of request.LastName.
                RegistrationDate = DateTime.Now,
                BirthDate = request.BirthDate,
                Gender = request.Gender,
                Score = request.Score ?? 0,
                StatusId = request.StatusId ?? 0,
                // ??: Null coalescing operator: If request.StatusId is null
                // assign 0 else assign request.StatusId value to the User entity
                // StatusId property. request.StatusId.Value may also be used
                // since request.StatusId is required.
                // Same for request.Score.
                RoleIds = request.RoleIds
            };
            DbAdd(userEntity);

            // Entity's Id value will be updated by the database
            // after the insert operation.
            return Success(userEntity.Id, "User created successfully.");
        }

        // Checks whether the user with the same trimmed and
        // case sensitive user name other than the user request record
        // exists in the Users database table first.
        // If the entity doesn't exist, gets the User entity by ID
        // from the table, deletes the relational UserRole entities,
        // then updates the User entity properties from user request
        // properties and finally updates the User entity in the
        // Users database table.
        public CommandResponse Update(UserRequest request)
        {
            if (DbQuery().Any(u => u.Id != request.Id
                && u.UserName == request.UserName.Trim()))
                return Error("User with the same user name exists!");

            // Get the User entity by ID from the Users database
            // table and check whether it is null or not.
            var userEntity = DbSingle(request.Id);
            if (userEntity is null)
                return Error("User not found!");

            // Delete the relational UserRole entities first.
            if (request.RoleIds is not null)
                DbRemove(userEntity.UserRoles);

            // Then update User entity properties from user request properties.
            // Then update the entity in the Users database table (second save
            // default parameter of the DbUpdate method is true
            // therefore changes of the DbSets are commited to the related
            // database tables).
            userEntity.UserName = request.UserName.Trim();
            userEntity.Password = request.Password.Trim();
            userEntity.FirstName = request.FirstName?.Trim();
            // Since request.FirstName may be null, ? is used after
            // meaning that if request.FirstName is null, assign
            // null to the User entity's FirstName property else
            // assign the trimmed value of request.FirstName.
            userEntity.LastName = request.LastName?.Trim();
            // Since request.LastName may be null, ? is used after
            // meaning that if request.LastName is null, assign
            // null to the User entity's LastName property else
            // assign the trimmed value of request.LastName.
            userEntity.BirthDate = request.BirthDate;
            userEntity.Gender = request.Gender;
            if (request.Score.HasValue)
                userEntity.Score = request.Score.Value;
            if (request.StatusId.HasValue)
                userEntity.StatusId = request.StatusId.Value;
            if (request.RoleIds is not null)
                userEntity.RoleIds = request.RoleIds;
            DbUpdate(userEntity);

            return Success(userEntity.Id, "User updated successfully.");
        }

        // Gets the User entity by ID from the Users database table.
        // If the entity exists, deletes the relational UserRole entities.
        // Then deletes the entity from the Users database table.
        public CommandResponse Remove(int id)
        {
            // Get the User entity by ID from the Users database table
            // and check whether it is null or not.
            var userEntity = DbSingle(id);
            if (userEntity is null)
                return Error("User not found!");

            // Delete the relational UserRole entities first even Cascade
            // delete rule is defined in the database (recommended).
            // This way would also be suitable if the delete rule was
            // defined as No Action in the database.
            DbRemove(userEntity.UserRoles);

            // Then delete the User entity from the Users database table
            // (second save default parameter of the DbRemove method is
            // true therefore changes of the DbSets are commited to
            // the related database tables).
            DbRemove(userEntity);

            return Success(userEntity.Id, "User deleted successfully.");
        }
    }
}
