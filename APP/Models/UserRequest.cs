using APP.Domain;
using CORE.Domain;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APP.Models
{
    // Request properties are created according to the data that
    // will be retrieved from APIs or UIs (views).
    public class UserRequest : Record
    {
        // Copy all the non navigation properties from User entity.
        /*
        ErrorMessage parameter can be set in all data annotations 
        to show custom validation error messages:  
        Example 1: [Required(ErrorMessage = "{0} is required!")] 
        where {0} is the DisplayName if defined otherwise property name
        which is "User Name".
        Example 2: [StringLength(30, 3, 
        ErrorMessage = "{0} must be minimum {2} maximum {1} characters!")] 
        where {0} is the DisplayName if defined otherwise property name 
        which is "User Name", {1} is the first parameter which is 30 and 
        {2} is the second parameter which is 3.
        */
        // User Name is required and can be minimum 3 maximum 30 characters.
        [Required(ErrorMessage = "{0} is required!")] 
        [StringLength(30, MinimumLength = 3, 
        ErrorMessage = "{0} must be minimum {2} maximum {1} characters!")]
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [StringLength(15, MinimumLength = 3,
        ErrorMessage = "{0} must be minimum {2} maximum {1} characters!")]
        public string Password { get; set; }
                
        [StringLength(50, ErrorMessage = "{0} must be maximum {1} characters!")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "{0} must be maximum {1} characters!")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        public Genders Gender { get; set; }

        [DisplayName("Birth Date")]
        public DateTime? BirthDate { get; set; }

        // We don't need to get the RegistrationDate value from the client
        // since it will be assigned automatically in the service.

        // Minimum value can be 0, maximum value can be 5.
        /*
        The type changed from double to double? to be able to use [Required] 
        attribute. If the type is double, the default value will be 0 and 
        the validation will always be successful even if no value is assigned.
        */
        [Required(ErrorMessage = "{0} is required!")]
        [Range(0, 5, ErrorMessage = "{0} must be between {1} and {2}!")] 
        public double? Score { get; set; }

        // We don't need to get the IsOnline value from the client
        // since it will be assigned automatically during login and logout.

        

        // Modify (1 to many) or add (many to many) the relationship properties.
        /* 
        The type changed from int to int? to be able to use [Required] attribute. 
        If the type is int, the default value will be 0 and the validation will 
        always be successful even if no value is assigned. Also we wan't to get a 
        null value from the drop down list (select HTML tag) when the 
        "-- Select --" option is selected, therefore the application user must 
        select a status option other than "-- Select --".
        */
        // For users-status one to many relationship.
        [Required(ErrorMessage = "{0} is required!")]
        [DisplayName("Status")]
        public int? StatusId { get; set; }

        // Required may not be defined if a user may not have a role.
        // For users-roles many to many relationship.
        [Required(ErrorMessage = "At least one role is required!")]
        [DisplayName("Roles")]
        public List<int> RoleIds { get; set; }
    }
}
