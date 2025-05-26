using Microsoft.EntityFrameworkCore;
using StarWarsAPI.Application.DTOs;
using StarWarsAPI.Application.Interfaces.IRepositories;
using StarWarsAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarWarsAPI.Infrastructure.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly StarWarsDbContext _context;

        public MovieRepository(StarWarsDbContext context)
        {
            _context = context;
        }

        public async Task<List<int>> GetExistingEpisodeIdsAsync()
        {
            return await _context.Movies
                .Select(m => m.EpisodeId)
                .ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Movie> movies)
        {
            await _context.Movies.AddRangeAsync(movies);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Movie>> GetAllAsync()
        {
            return await _context.Movies.AsNoTracking().ToListAsync();
        }

        public async Task<Movie> GetByIdAsync(int id)
        {
            return await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(Movie movie)
        {
            await _context.Movies.AddAsync(movie);
        }

        public void Delete(Movie movie)
        {
            _context.Movies.Remove(movie);
        }

    }
}
