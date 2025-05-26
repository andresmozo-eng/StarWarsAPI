using AutoMapper;
using StarWarsAPI.Application.DTOs;
using StarWarsAPI.Application.DTOs.Swapi;
using StarWarsAPI.Application.Exceptions;
using StarWarsAPI.Application.Interfaces.IRepositories;
using StarWarsAPI.Application.Interfaces.IServices;
using StarWarsAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace StarWarsAPI.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly HttpClient _httpClient;
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;

        public MovieService(HttpClient httpClient, IMovieRepository movieRepository, IMapper mapper)
        {
            _httpClient = httpClient;
            _movieRepository = movieRepository;
            _mapper = mapper;
        }

        public async Task SyncMoviesAsync()
        {
            var response = await _httpClient.GetAsync("https://www.swapi.tech/api/films");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var swapiResponse = JsonSerializer.Deserialize<SwapiFilmListResponseDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (swapiResponse?.Result == null || !swapiResponse.Result.Any())
                return;

            var existingEpisodeIds = await _movieRepository.GetExistingEpisodeIdsAsync();

            var newMovies = swapiResponse.Result
                .Where(f => !existingEpisodeIds.Contains(f.Properties.EpisodeId))
                .Select(f => new Movie
                {
                    EpisodeId = f.Properties.EpisodeId,
                    Title = f.Properties.Title,
                    OpeningCrawl = f.Properties.OpeningCrawl,
                    Director = f.Properties.Director,
                    Producer = f.Properties.Producer,
                    ReleaseDate = f.Properties.ReleaseDate
                })
                .ToList();

            if (newMovies.Any())
            {
                await _movieRepository.AddRangeAsync(newMovies);
                await _movieRepository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<MovieResponseDto>> GetAllMoviesAsync()
        {
            var movies = await _movieRepository.GetAllAsync();

            return _mapper.Map<List<MovieResponseDto>>(movies);
        }

        public async Task<MovieResponseDto> GetMovieByIdAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
                return null;

            return _mapper.Map<MovieResponseDto>(movie);
        }

        public async Task<MovieResponseDto> CreateMovieAsync(CreateMovieDto dto)
        {
            var movie = _mapper.Map<Movie>(dto);

            await _movieRepository.AddAsync(movie);
            await _movieRepository.SaveChangesAsync();

            return _mapper.Map<MovieResponseDto>(movie);
        }

        public async Task UpdateMovieAsync(int id, UpdateMovieDto dto)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
                throw new NotFoundException("Película no encontrada");

            movie.Title = dto.Title;
            movie.EpisodeId = dto.EpisodeId;
            movie.OpeningCrawl = dto.OpeningCrawl;
            movie.Director = dto.Director;
            movie.Producer = dto.Producer;
            movie.ReleaseDate = dto.ReleaseDate;

            await _movieRepository.SaveChangesAsync();
        }

        public async Task DeleteMovieAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);

            if (movie == null)
                throw new NotFoundException($"No se encontró la película con ID {id}.");

            _movieRepository.Delete(movie);
            await _movieRepository.SaveChangesAsync();
        }





    }
}
