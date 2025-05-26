using Xunit;
using Moq;
using AutoMapper;
using StarWarsAPI.API.Controllers;
using StarWarsAPI.Application.Interfaces.IServices;
using StarWarsAPI.API.Models.Requests;
using StarWarsAPI.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace StarWarsAPI.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _mapperMock = new Mock<IMapper>();
            _controller = new UserController(_userServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Register_ReturnsOk_WithSuccessMessage()
        {
            // Arrange
            var request = new RegisterUserRequest { UserName = "testuser", Password = "password" , Email = "a@a.com" };
            var dto = new RegisterUserDto { UserName = "testuser", Password = "password" , Email = "a@a.com" };

            _mapperMock.Setup(m => m.Map<RegisterUserDto>(request)).Returns(dto);
            _userServiceMock.Setup(s => s.RegisterUserAsync(dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var message = okResult.Value.GetType().GetProperty("Message").GetValue(okResult.Value, null);
            Assert.Equal("Usuario registrado con éxito", message);
        }

        [Fact]
        public async Task Login_ReturnsOk_WithToken()
        {
            // Arrange
            var request = new LoginUserRequest { Email = "testuser@email.com", Password = "password" };
            var dto = new LoginUserDto { Email = "testuser@EMAIL.COM", Password = "password" };
            var token = "jwt_token_example";

            _mapperMock.Setup(m => m.Map<LoginUserDto>(request)).Returns(dto);
            _userServiceMock.Setup(s => s.LoginUserAsync(dto)).ReturnsAsync(token);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var tokenValue = okResult.Value.GetType().GetProperty("Token").GetValue(okResult.Value, null);
            Assert.Equal(token, tokenValue);
        }
    }
}