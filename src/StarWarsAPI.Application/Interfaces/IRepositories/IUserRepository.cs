using StarWarsAPI.Domain.Entities;
using System.Threading.Tasks;

namespace StarWarsAPI.Application.Interfaces.IRepositories
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<bool> ExistsAsync(string email);
        Task SaveChangesAsync();
    }
}
