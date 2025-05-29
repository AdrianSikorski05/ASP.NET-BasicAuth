namespace RestFullApiTest
{
    public class RefreshToken
    {
        /// <summary>
        /// Unique identifier for the refresh token
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The identifier of the user associated with the refresh token
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// The refresh token string used for authentication
        /// </summary>
        public string Token { get; set; } = null!;

        /// <summary>
        /// The date and time when the refresh token expires
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Indicates whether the refresh token has been revoked
        /// </summary>
        public bool Revoked { get; set; }
    }
}
