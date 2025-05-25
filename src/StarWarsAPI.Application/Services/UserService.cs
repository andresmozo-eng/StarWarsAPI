using AutoMapper;
using StarWarsAPI.Application.DTOs;
using StarWarsAPI.Application.Helpers;
using StarWarsAPI.Application.Interfaces.IRepositories;
using StarWarsAPI.Application.Interfaces.IServices;
using StarWarsAPI.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace StarWarsAPI.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasherHelper _passwordHasher;

        public UserService(IUserRepository userRepository, IMapper mapper , IPasswordHasherHelper passwordHasher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task RegisterUserAsync(RegisterUserDto dto)
        {
            if (await _userRepository.ExistsAsync(dto.Email))
                throw new ArgumentException("El email ya está registrado.");

            _passwordHasher.CreatePasswordHash(dto.Password, out var hash, out var salt);
            var user = new User(dto.UserName, dto.Email, hash, salt);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

        }
    }
}
