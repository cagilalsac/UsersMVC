using CORE.Domain;
using System.ComponentModel;

namespace APP.Models
{
    // Response properties are created according to the data to be
    // presented in API responses or UIs (Views).
    public class StatusResponse : Record
    {
        // Copy all the non navigation properties from Status entity.
        public string Title { get; set; }



        // Add the new properties ending with R declaring Response
        // for custom properties or formatted string value properties.
        // Way 1: Map primitive type properties for basic information.
        [DisplayName("Users")]
        public string UserNamesR { get; set; }

        // Property value for the count of the users of the status.
        [DisplayName("User Count")]
        public int UserCountR { get; set; }

        // Way 2: Map complex type object for more information.
        // Either Way 1, Way 2 or both can be used.
        [DisplayName("Users")]
        public List<UserResponse> UsersR { get; set; }
    }
}
