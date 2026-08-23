using CORE.Domain;
using CORE.Models;

namespace CORE.Services
{
    // Interface to perform CRUD (Create, Read, Update, Delete)
    // operations for specific request and response types
    // inherited from the Record class and have a parameterless
    // constructor. These methods will be used within the controller
    // actions after the implemented service class injection and
    // will implement business logic such as querying and projecting
    // (mapping) entity data to response data, validation in database
    // table, mapping request data to entity data then inserting and
    // updating the entity data in the database table, deleting entity
    // data by ID from the database table, etc.
    public interface IService<TRequest, TResponse> 
        where TRequest : Record, new() 
        where TResponse : Record, new()
    {
        // Returns a projected query of type TResponse from an entity.
        // public may not be written.
        public IQueryable<TResponse> Query(); 

        // Returns a list of TResponse from the Query method.
        public List<TResponse> List() => Query().ToList();

        // Returns an item of TResponse by ID from the Query method.
        public TResponse Single(int id) 
            => Query().SingleOrDefault(response => response.Id == id);

        // Returns an item of TRequest by ID.
        public TRequest Edit(int id);

        // Maps the request to the entity and inserts it to the database table.
        public CommandResponse Add(TRequest request);

        // Maps the request to the entity and updates it in the database table.
        public CommandResponse Update(TRequest request);

        // Deletes the entity from the database table by ID.
        public CommandResponse Remove(int id);
    }
}
