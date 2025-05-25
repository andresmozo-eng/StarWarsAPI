using StarWarsAPI.Application.DTOs;
using System.Threading.Tasks;

namespace StarWarsAPI.Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task RegisterUserAsync(RegisterUserDto registerUserDto);
    }
}
