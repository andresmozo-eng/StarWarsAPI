namespace StarWarsAPI.API.Models.Requests
{
    public class LoginUserRequest
    {
        /// <example>usuario@email.com</example>
        public string Email { get; set; }

        /// <example>MySecurePassword123!</example>
        public string Password { get; set; }
    }
}
