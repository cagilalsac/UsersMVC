using System.ComponentModel;

namespace APP.Models
{
    /// <summary>
    /// The model to be used with UserSessionService to
    /// represent user session data with minimum, maximum
    /// and average score calculations.
    /// </summary>
    public class UserSessionResponse
    {
        /// <summary>
        /// The user ID of the user who is logged in.
        /// </summary>  
        public int AuthenticatedUserId { get; set; }

        /// <summary>
        /// The user ID of the user whose data is being retrieved.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The user name of the user whose data is being retrieved.
        /// </summary>
        [DisplayName("User Name")]
        public string UserName { get; set; }

        /// <summary>
        /// The score of the user whose data is being retrieved.
        /// Will be used to calculate minimum, maximum and average
        /// scores.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// The score text of the user whose data is being retrieved.
        /// Also will be used to display minimum, maximum and average
        /// score calculations.
        /// </summary>
        [DisplayName("Score")]
        public string ScoreText { get; set; }
    }
}
