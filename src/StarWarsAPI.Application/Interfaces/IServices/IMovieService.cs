using StarWarsAPI.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarWarsAPI.Application.Interfaces.IServices
{
    public interface IMovieService
    {
        Task SyncMoviesAsync();
        Task<IEnumerable<MovieResponseDto>> GetAllMoviesAsync();
        Task<MovieResponseDto> GetMovieByIdAsync(int id);
        Task<MovieResponseDto> CreateMovieAsync(CreateMovieDto request);
        Task UpdateMovieAsync(int id, UpdateMovieDto request);
        Task DeleteMovieAsync(int id);
    }
}
