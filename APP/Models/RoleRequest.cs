using CORE.Domain;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APP.Models
{
    // Request properties are created according to the data that
    // will be retrieved from APIs or UIs (views).
    public class RoleRequest : Record
    {
        // Copy all the non navigation properties from Role entity.
        // Name can't be null and can be maximum 25 characters.
        [Required, StringLength(25)]
        public string Name { get; set; }



        // Modify (1 to many) or add (many to many) the relationship properties.
        // Since we won't update the relational user data (UserRoles)
        // through Role request, we don't need the UserIds property here.
        // Required may not be defined if a role may not have a user.
        // For roles-users many to many relationship.
        //[Required]
        //[DisplayName("Users")]
        //public List<int> UserIds { get; set; } = new();
    }
}
