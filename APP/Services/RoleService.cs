using APP.Domain;
using APP.Models;
using CORE.Models;
using CORE.Services;
using Microsoft.EntityFrameworkCore;

namespace APP.Services
{
    // RoleService inherits the DbService class for Role entity type,
    // therefore Role entity database operations of the DbService
    // class can be used in this class.
    // RoleService also implements the IService interface for
    // RoleRequest and RoleResponse types, therefore IService method
    // definitions can be implemented for types RoleRequest and
    // RoleResponse in this class, which will be used by
    // the controller actions.
    public class RoleService : DbService<Role>,
        IService<RoleRequest, RoleResponse>
    {
        // Constructor that passes the injected DbContext instance
        // to the DbService base class constructor.
        public RoleService(DbContext db) : base(db)
        {
        }

        // Gets the Role entity query then projects the Role
        // entity query to the RoleResponse query and returns it.
        // Example query: "select Name from Roles".
        // where Name is the RoleResponse property.
        // The returned query can be executed by invoking
        // ToList, SingleOrDefault, FirstOrDefault, Any,
        // etc. LINQ (Language Integrated Query) methods
        // and then the returned result can be used.
        // This projected query by the Select LINQ method
        // will be used by the List and Single methods of the
        // IService interface.
        public IQueryable<RoleResponse> Query()
        {
            return DbQuery() // "select * from Roles" entity query.
                .Select(roleEntity => new RoleResponse() 
                // () after the class name may not be written.
                {
                    Id = roleEntity.Id,
                    Name = roleEntity.Name
                }); // Map each Role entity property
                    // to RoleResponse property.
        }

        // Gets the Role entity by ID then returns a mapped
        // RoleRequest instance from the found Role entity.
        public RoleRequest Edit(int id)
        {
            var roleEntity = DbSingle(id);
            // Example SQL: "select * from Roles where Id = 3".

            // roleEntity may be null since the OrDefault version
            // of the Single method is used. If Single method
            // was used, an exception would be thrown.
            if (roleEntity is null) // if (roleEntity == null) can
                                    // also be written.
                return null;

            // Map each Role entity property to RoleRequest property
            // and return the RoleRequest instance.
            return new RoleRequest
            {
                Id = roleEntity.Id,
                Name = roleEntity.Name
            };
        }

        // Checks whether the role with the same trimmed and
        // case sensitive name exists in the Roles database table
        // first. If it doesn't exist, inserts the Role entity
        // mapped from the RoleRequest instance to the Roles
        // database table.
        public CommandResponse Add(RoleRequest request)
        {
            if (DbQuery()
                .Any(roleEntity =>
                    roleEntity.Name == request.Name.Trim()))
                return Error("\"" + request.Name + "\"" + " role exists!");
            // Trim method removes white space characters in the
            // beginning and at the end.

            // Create a new Role entity instance from the
            // RoleRequest instance then insert the entity in the Roles
            // database table (second save default parameter of the
            // DbAdd method is true therefore changes of the DbSets
            // are commited to the related database tables).
            var roleEntity = new Role
            {
                Name = request.Name.Trim()
            };
            DbAdd(roleEntity);

            // Entity's Id value will be updated by the database
            // after the insert operation.
            return Success(roleEntity.Id, "\"" + roleEntity.Name + "\"" + 
                " role created successfully.");
        }

        // Checks whether the role with the same trimmed and
        // case sensitive name other than the role request record
        // exists in the Roles database table first.
        // If the entity doesn't exist, gets the Role entity by ID from the
        // table, then updates the Role entity properties
        // from role request properties and finally updates the
        // Role entity in the Roles database table.
        public CommandResponse Update(RoleRequest request)
        {
            if (DbQuery()
                .Any(roleEntity => roleEntity.Id != request.Id &&
                    roleEntity.Name == request.Name.Trim()))
                return Error($"\"{request.Name}\" role exists!");
            // $ with strings can be used for concatenation with { and }.
            
            // Get the Role entity by ID from the Roles database table
            // and check whether it is null or not.
            var roleEntity = DbSingle(request.Id);
            if (roleEntity is null)
                return Error("Role not found!");

            // Update Role entity properties from role request properties.
            // Then update the entity in the Roles database table (second save
            // default parameter of the DbUpdate method is true
            // therefore changes of the DbSets are commited to the related
            // database tables).
            roleEntity.Name = request.Name.Trim();
            DbUpdate(roleEntity);

            return Success(roleEntity.Id, 
                $"\"{roleEntity.Name}\" role updated successfully.");
        }

        // Gets the Role entity by ID from the Roles database table.
        // If the entity exists, deletes the entity from the Roles
        // database table.
        public CommandResponse Remove(int id)
        {
            // Get the Role entity by ID from the Roles database table
            // and check whether it is null or not.
            var roleEntity = DbSingle(id);
            if (roleEntity is null)
                return Error("Role not found!");

            // Delete the Role entity from the Roles database table
            // (second save default parameter of the DbRemove method is
            // true therefore changes of the DbSets are commited to
            // the related database tables).
            // Since the delete rule of the Roles and UserRoles relation
            // in the database is Cascade, the relational UserRole entities
            // will be deleted automatically (not recommended).
            DbRemove(roleEntity);

            return Success(roleEntity.Id,
                $"\"{roleEntity.Name}\" role deleted successfully.");
        }
    }
}
