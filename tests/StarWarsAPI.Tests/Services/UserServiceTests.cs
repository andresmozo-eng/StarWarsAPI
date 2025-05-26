using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using StarWarsAPI.Application.DTOs;
using StarWarsAPI.Application.Enums;
using StarWarsAPI.Application.Exceptions;
using StarWarsAPI.Application.Helpers;
using StarWarsAPI.Application.Interfaces.IRepositories;
using StarWarsAPI.Application.Services;
using StarWarsAPI.Application.Settings;
using StarWarsAPI.Domain.Entities;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StarWarsAPI.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IPasswordHasherHelper> _passwordHasherMock;
        private readonly JwtSettings _jwtSettings;
        private readonly Mock<IOptions<JwtSettings>> _jwtOptionsMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _passwordHasherMock = new Mock<IPasswordHasherHelper>();
            _jwtSettings = new JwtSettings
            {
                SecretKey = "supersecretkeysupersecretkey123!",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpirationInMinutes = 60
            };
            _jwtOptionsMock = new Mock<IOptions<JwtSettings>>();
            _jwtOptionsMock.Setup(j => j.Value).Returns(_jwtSettings);

            _userService = new UserService(
                _userRepositoryMock.Object,
                _mapperMock.Object,
                _passwordHasherMock.Object,
                _jwtOptionsMock.Object
            );
        }

        [Fact]
        public async Task RegisterUserAsync_Should_Add_User_When_Email_Is_Not_Registered()
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Email = "test@email.com",
                Password = "password123",
                UserName = "testuser"
            };
            byte[] hash = Encoding.UTF8.GetBytes("hash");
            byte[] salt = Encoding.UTF8.GetBytes("salt");

            _userRepositoryMock.Setup(r => r.ExistsAsync(dto.Email)).ReturnsAsync(false);
            _passwordHasherMock.Setup(p => p.CreatePasswordHash(dto.Password, out hash, out salt));
            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _userRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _userService.RegisterUserAsync(dto);

            // Assert
            _userRepositoryMock.Verify(r => r.ExistsAsync(dto.Email), Times.Once);
            _passwordHasherMock.Verify(p => p.CreatePasswordHash(dto.Password, out hash, out salt), Times.Once);
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
            _userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_Should_Throw_ArgumentException_When_Email_Exists()
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Email = "existing@email.com",
                Password = "password",
                UserName = "user1"
            };

            _userRepositoryMock.Setup(r => r.ExistsAsync(dto.Email)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _userService.RegisterUserAsync(dto));
        }

        [Fact]
        public async Task LoginUserAsync_Should_Return_Token_When_Credentials_Are_Valid()
        {
            // Arrange
            var dto = new LoginUserDto
            {
                Email = "user@email.com",
                Password = "password"
            };
            var user = new User("testuser", dto.Email, new byte[] { 1, 2 }, new byte[] { 3, 4 }, (int)RoleType.User)
            {
                Role = new Role { Id = (int)RoleType.User, Description = "User" }
            };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);
            _passwordHasherMock.Setup(p => p.VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
                .Returns(true);

            // Act
            var token = await _userService.LoginUserAsync(dto);

            // Assert
            Assert.False(string.IsNullOrEmpty(token));
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.Contains(jwtToken.Claims, c => c.Type == "nameid" && c.Value == user.Id.ToString());
            Assert.Contains(jwtToken.Claims, c => c.Type == "unique_name" && c.Value == user.UserName);
            Assert.Contains(jwtToken.Claims, c => c.Type == "email" && c.Value == user.Email);
            Assert.Contains(jwtToken.Claims, c => c.Type == "role" && c.Value == user.Role.Description);
        }

        [Fact]
        public async Task LoginUserAsync_Should_Throw_NotFoundException_When_User_Not_Found()
        {
            // Arrange
            var dto = new LoginUserDto
            {
                Email = "notfound@email.com",
                Password = "password"
            };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _userService.LoginUserAsync(dto));
        }

        [Fact]
        public async Task LoginUserAsync_Should_Throw_InvalidCredentialsException_When_Password_Wrong()
        {
            // Arrange
            var dto = new LoginUserDto
            {
                Email = "user@email.com",
                Password = "wrongpassword"
            };
            var user = new User("testuser", dto.Email, new byte[] { 1, 2 }, new byte[] { 3, 4 }, (int)RoleType.User);

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);
            _passwordHasherMock.Setup(p => p.VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
                .Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCredentialsException>(() => _userService.LoginUserAsync(dto));
        }
    }
}