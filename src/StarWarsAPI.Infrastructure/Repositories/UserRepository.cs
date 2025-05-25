using Microsoft.EntityFrameworkCore;
using StarWarsAPI.Application.Interfaces.IRepositories;
using StarWarsAPI.Domain.Entities;
using System.Threading.Tasks;

namespace StarWarsAPI.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly StarWarsDbContext _context;

        public UserRepository(StarWarsDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}
