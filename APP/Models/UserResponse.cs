using APP.Domain;
using CORE.Domain;
using System.ComponentModel;

namespace APP.Models
{
    // Response properties are created according to the data to be
    // presented in API responses or UIs (Views).
    public class UserResponse : Record
    {
        // Copy all the non navigation properties from User entity.
        // DisplayName attribute is used to write the title
        // for the property in the view instead of the property
        // name, so "User Name" will be written in the view instead
        // of "UserName". If no DisplayName attribute is defined,
        // the property name will be written in the view.
        // Therefore, DisplayName attribute can be used to show user
        // friendly names in both validation error messages and views
        // using DisplayNameFor HTML Helper.
        [DisplayName("User Name")]
        public string UserName { get; set; }
        
        public string Password { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }
        
        public Genders Gender { get; set; }

        public DateTime? BirthDate { get; set; }
        
        public DateTime RegistrationDate { get; set; }
        
        public double Score { get; set; }

        public bool IsOnline { get; set; }

        public int StatusId { get; set; }



        // Add the new properties ending with R declaring Response
        // for custom properties or formatted string value properties.
        [DisplayName("Online")]
        public string IsOnlineR { get; set; }

        [DisplayName("Score")]
        public string ScoreR { get; set; }

        [DisplayName("Registration Date")]
        public string RegistrationDateR { get; set; }

        [DisplayName("Birth Date")]
        public string BirthDateR { get; set; }

        [DisplayName("Gender")]
        public string GenderR { get; set; }

        // FirstName + " " + LastName concatenated value.
        [DisplayName("Full Name")]
        public string FullNameR { get; set; }

        // Password value hidden with * characters.
        [DisplayName("Password")]
        public string PasswordR { get; set; }

        // Way 1:
        // Property for the Title value of the
        // related Status entity.
        [DisplayName("Status")]
        public string StatusTitleR { get; set; }

        // Way 2:
        // Property for the mapped StatusResponse object
        // from the related Status entity.
        // Either Way 1, Way 2 or both can be used.
        [DisplayName("Status")]
        public StatusResponse StatusR { get; set; }

        // Way 1:
        // Property for the concatenated role names
        // of related Role entities.
        [DisplayName("Roles")]
        public string RoleNamesR { get; set; }

        [DisplayName("Roles")]
        // Way 2:
        // Property for the RoleResponse objects projected
        // from the related Role entities.
        // Either Way 1, Way 2 or both can be used.
        public List<RoleResponse> RolesR { get; set; }
    }
}
