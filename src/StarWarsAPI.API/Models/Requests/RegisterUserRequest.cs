using System.ComponentModel.DataAnnotations;

namespace StarWarsAPI.API.Models.Requests
{
    public class RegisterUserRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

        [Required]
        public string UserName { get; set; } = null!;
    }
}
