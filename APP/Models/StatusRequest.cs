using CORE.Domain;
using System.ComponentModel.DataAnnotations;

namespace APP.Models
{
    // Request properties are created according to the data that
    // will be retrieved from APIs or UIs (views).
    public class StatusRequest : Record
    {
        // Copy all the non navigation properties from Status entity.
        [Required, StringLength(5)]
        public string Title { get; set; }
    }
}
