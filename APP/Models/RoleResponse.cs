using CORE.Domain;

namespace APP.Models
{
    // Response properties are created according to the data to be
    // presented in API responses or UIs (Views).
    public class RoleResponse : Record
    {
        // Copy all the non navigation properties from Role entity.
        public string Name { get; set; }
    }
}
