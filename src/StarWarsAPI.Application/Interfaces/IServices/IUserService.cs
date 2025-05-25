using StarWarsAPI.Application.DTOs;
using System.Threading.Tasks;

namespace StarWarsAPI.Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task<string> LoginUserAsync(LoginUserDto loginUserDto);
        Task RegisterUserAsync(RegisterUserDto registerUserDto);
    }
}
