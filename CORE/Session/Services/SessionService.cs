using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;

namespace CORE.Session.Services
{
    /// <summary>
    /// Concrete class for session management.
    /// Provides methods to store, retrieve, and remove 
    /// complex objects in session using JSON serialization.
    /// Implemented as a concrete class because the behaviors
    /// of this class are standard and are not expected to change.
    /// </summary>
    public class SessionService
    {
        /// <summary>
        /// Provides access to the current HTTP context, 
        /// enabling session operations.
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes the session service with an 
        /// injected IHttpContextAccessor instance.
        /// </summary>
        /// <param name="httpContextAccessor">Accessor for the current 
        /// HTTP context.</param>
        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Readonly property to get the authenticated user's ID value.
        /// </summary>
        public int AuthenticatedUserId => Convert.ToInt32(
            _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(
                claim => claim.Type == "Id").Value);

        /// <summary>
        /// Retrieves a session value by key and deserializes it to the 
        /// specified type.
        /// Returns null if the key does not exist or the string value 
        /// is empty.</summary>
        /// <typeparam name="T">Type to deserialize the session string 
        /// value to.</typeparam>
        /// <param name="key">Unique session key.</param>
        /// <returns>Deserialized object or null.</returns>
        public T Get<T>(string key) 
            where T : class // T can be any reference type.
        {
            var value = _httpContextAccessor.HttpContext
                .Session.GetString(key);
            // Retrieves the JSON string value from session by key.
            if (string.IsNullOrEmpty(value))
                return null;
            return JsonSerializer.Deserialize<T>(value); 
            // Converts JSON string to object of type T and returns it.
        }

        /// <summary>
        /// Serializes the provided object and stores it in session under 
        /// the specified key.
        /// Does nothing if the instance is null.
        /// </summary>
        /// <typeparam name="T">Type of the object to store.</typeparam>
        /// <param name="key">Unique session key.</param>
        /// <param name="instance">Object to serialize and store.</param>
        public void Set<T>(string key, T instance) 
            where T : class // T can be any reference type.
        {
            if (instance is not null)
            {
                var value = JsonSerializer.Serialize(instance); 
                // Converts object of type T to JSON string.
                _httpContextAccessor.HttpContext.Session.SetString(key, value);
                // Stores the JSON string value in session under
                // the specified key.
            }
        }

        /// <summary>
        /// Removes the session value associated with the specified key.
        /// </summary>
        /// <param name="key">Unique session key to remove.</param>
        public void Remove(string key)
        {
            _httpContextAccessor.HttpContext.Session.Remove(key);
            // Removes the session value associated with the specified key.
        }
    }
}
