namespace StarWarsAPI.API.Models.Requests
{
    public class RegisterUserRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string UserName { get; set; } = null!;
    }
}
