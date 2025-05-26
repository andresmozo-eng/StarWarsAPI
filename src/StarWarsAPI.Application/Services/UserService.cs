using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StarWarsAPI.Application.DTOs;
using StarWarsAPI.Application.Enums;
using StarWarsAPI.Application.Exceptions;
using StarWarsAPI.Application.Helpers;
using StarWarsAPI.Application.Interfaces.IRepositories;
using StarWarsAPI.Application.Interfaces.IServices;
using StarWarsAPI.Application.Settings;
using StarWarsAPI.Domain.Entities;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StarWarsAPI.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasherHelper _passwordHasher;
        private readonly JwtSettings _jwtSettings;

        public UserService(IUserRepository userRepository, IMapper mapper , IPasswordHasherHelper passwordHasher , IOptions<JwtSettings> jwtOptions)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task RegisterUserAsync(RegisterUserDto dto)
        {
            if (await _userRepository.ExistsAsync(dto.Email))
                throw new ArgumentException("El email ya está registrado.");

            _passwordHasher.CreatePasswordHash(dto.Password, out var hash, out var salt);
            var user = new User(dto.UserName, dto.Email, hash, salt, (int)RoleType.User); //Created as user by default

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

        }

        public async Task<string> LoginUserAsync(LoginUserDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null)
                throw new NotFoundException("Usuario no encontrado.");

            var passwordIsValid = _passwordHasher.VerifyPasswordHash(
                loginDto.Password, user.PasswordHash, user.PasswordSalt);

            if (!passwordIsValid)
                throw new InvalidCredentialsException("Credenciales inválidas.");

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Description)
            }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
