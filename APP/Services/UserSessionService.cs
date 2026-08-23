using APP.Models;
using CORE.Services;
using CORE.Session.Services;

namespace APP.Services
{
    /// <summary>
    /// Concrete class for managing user session data.
    /// Provides methods to add, remove, retrieve, and 
    /// clear user session data.
    /// Implemented as a concrete class because the behaviors
    /// of this class are standard and are not expected to change.
    /// </summary>
    public class UserSessionService
    {
        // Constant key value to store and retrieve user session
        // data in the session.
        // Default private if no accessibility modifier is specified.
        const string KEY = "UserSession";

        // Services to be injected for user session data management.
        private readonly SessionService _sessionService;
        private readonly IService<UserRequest, UserResponse> _userService;

        public UserSessionService(SessionService sessionService,
            IService<UserRequest, UserResponse> userService)
        {
            _sessionService = sessionService;
            _userService = userService;
        }

        /// <summary>
        /// Retrieves the list of user session data for the specified 
        /// authenticated user. Then calculates the minimum, maximum 
        /// and average scores of the users if there is at least one
        /// user. Finally adds a calculation summary item to the list 
        /// before returning it.
        /// </summary>
        /// <returns>List of user session response items including 
        /// a summary item. If no users data is found in the session,
        /// returns an empty list.</returns>
        public List<UserSessionResponse> Get()
        {
            var sessionUsers = _sessionService
                .Get<List<UserSessionResponse>>(KEY);
            if (sessionUsers is not null)
            {
                sessionUsers = sessionUsers.Where(
                    su => su.AuthenticatedUserId 
                        == _sessionService.AuthenticatedUserId)
                            .ToList();
                if (sessionUsers.Count > 0)
                {
                    var minimumScoreText = sessionUsers
                        .Min(su => su.Score).ToString("N1");
                    var maximumScoreText = sessionUsers
                        .Max(su => su.Score).ToString("N1");
                    var averageScoreText = sessionUsers
                        .Average(su => su.Score).ToString("N1");
                    sessionUsers.Add(new UserSessionResponse
                    {
                        ScoreText = $"<b>Minimum Score:</b> {minimumScoreText}, " +
                                    $"<b>Maximum Score:</b> {maximumScoreText}, " +
                                    $"<b>Average Score:</b> {averageScoreText}"
                    });
                }
                return sessionUsers;
            }
            return new List<UserSessionResponse>();
        }

        /// <summary>
        /// Adds the user retrieved from the Users database table by
        /// the specified user ID parameter value if found and haven't been
        /// added to users session before. Then sets the updated user list 
        /// in the authenticated user's session.
        /// </summary>
        /// <param name="userId">User ID to get the user from the 
        /// Users database table.</param>
        public void Add(int userId)
        {
            var user = _userService.Single(userId);
            if (user is not null)
            {
                var sessionUsers = Get();
                if (!sessionUsers.Any(su => su.UserId == user.Id 
                    && su.AuthenticatedUserId == _sessionService.AuthenticatedUserId))
                {
                    sessionUsers.Add(new UserSessionResponse
                    {
                        AuthenticatedUserId = _sessionService.AuthenticatedUserId,
                        UserId = user.Id,
                        UserName = user.UserName,
                        Score = user.Score,
                        ScoreText = user.Score.ToString("N1")
                    });
                    _sessionService.Set(KEY, sessionUsers);
                }
            }
        }

        /// <summary>
        /// Removes the user from the authenticated users's session
        /// then updates the user list in the session.
        /// </summary>
        /// <param name="userId">User ID to remove from the session.</param>
        public void Remove(int userId)
        {
            var sessionUsers = Get();
            var sessionUser = sessionUsers
                .FirstOrDefault(su => su.UserId == userId
                    && su.AuthenticatedUserId == _sessionService.AuthenticatedUserId);
            if (sessionUser is not null)
            {
                sessionUsers.Remove(sessionUser);
                _sessionService.Set(KEY, sessionUsers);
            }
        }

        /// <summary>
        /// Removes all users from the authenticated user's session
        /// then updates the user list in the session.
        /// </summary>
        public void Clear()
        {
            var sessionUsers = Get();
            sessionUsers.RemoveAll(
                su => su.AuthenticatedUserId == _sessionService.AuthenticatedUserId);
            _sessionService.Set(KEY, sessionUsers);
        }
    }
}
