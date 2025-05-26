using StarWarsAPI.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarWarsAPI.Application.Interfaces.IServices
{
    public interface IMovieService
    {
        Task SyncMoviesAsync();
        Task<IEnumerable<MovieResponseDto>> GetAllMoviesAsync();
        Task<MovieResponseDto> GetMovieByIdAsync(int id);

    }
}
