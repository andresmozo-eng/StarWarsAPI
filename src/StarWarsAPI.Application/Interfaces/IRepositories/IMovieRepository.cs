using StarWarsAPI.Application.DTOs;
using StarWarsAPI.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarWarsAPI.Application.Interfaces.IRepositories
{
    public interface IMovieRepository
    {
        Task<List<int>> GetExistingEpisodeIdsAsync();
        Task AddRangeAsync(IEnumerable<Movie> movies);
        Task SaveChangesAsync();
        Task<IEnumerable<Movie>> GetAllAsync();
        Task<Movie> GetByIdAsync(int id);
        Task AddAsync(Movie movie);
    }
}
