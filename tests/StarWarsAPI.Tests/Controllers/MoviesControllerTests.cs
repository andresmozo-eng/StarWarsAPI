using Xunit;
using Moq;
using AutoMapper;
using StarWarsAPI.API.Controllers;
using StarWarsAPI.Application.Interfaces.IServices;
using StarWarsAPI.Application.DTOs;
using StarWarsAPI.API.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarWarsAPI.Tests.Controllers
{
    public class MoviesControllerTests
    {
        private readonly Mock<IMovieService> _movieServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly MoviesController _controller;

        public MoviesControllerTests()
        {
            _movieServiceMock = new Mock<IMovieService>();
            _mapperMock = new Mock<IMapper>();
            _controller = new MoviesController(_movieServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task SyncMovies_ReturnsOk_WithSuccessMessage()
        {
            // Arrange
            _movieServiceMock.Setup(s => s.SyncMoviesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SyncMovies();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var message = okResult.Value.GetType().GetProperty("Message").GetValue(okResult.Value, null);
            Assert.Contains("sincronizaron", message.ToString());
        }

        [Fact]
        public async Task GetMovies_ReturnsOk_WithListOfMovies()
        {
            // Arrange
            var movies = new List<MovieResponseDto>
            {
                new MovieResponseDto { Title = "A New Hope" }
            };
            _movieServiceMock.Setup(s => s.GetAllMoviesAsync())
                .ReturnsAsync(movies);

            // Act
            var result = await _controller.GetMovies();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(movies, okResult.Value);
        }

        [Fact]
        public async Task GetMovieById_ReturnsOk_WhenMovieExists()
        {
            // Arrange
            int movieId = 1;
            var movie = new MovieResponseDto { Title = "A New Hope" };
            _movieServiceMock.Setup(s => s.GetMovieByIdAsync(movieId))
                .ReturnsAsync(movie);

            // Act
            var result = await _controller.GetMovieById(movieId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(movie, okResult.Value);
        }

        [Fact]
        public async Task GetMovieById_ReturnsNotFound_WhenMovieDoesNotExist()
        {
            // Arrange
            int movieId = 2;
            _movieServiceMock.Setup(s => s.GetMovieByIdAsync(movieId))
                .ReturnsAsync((MovieResponseDto)null);

            // Act
            var result = await _controller.GetMovieById(movieId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            var message = notFoundResult.Value.GetType().GetProperty("Message").GetValue(notFoundResult.Value, null);
            Assert.Contains($"{movieId}", message.ToString());
        }

        [Fact]
        public async Task CreateMovie_ReturnsOkWithCreatedMovie()
        {
            // Arrange
            var request = new CreateMovieRequest { Title = "A New Hope" };
            var dto = new CreateMovieDto { Title = "A New Hope" };
            var created = new MovieResponseDto { Title = "A New Hope" };

            _mapperMock.Setup(m => m.Map<CreateMovieDto>(request)).Returns(dto);
            _movieServiceMock.Setup(s => s.CreateMovieAsync(dto)).ReturnsAsync(created);

            // Act
            var result = await _controller.CreateMovie(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode); 
            Assert.Equal(created, okResult.Value);
        }

        [Fact]
        public async Task UpdateMovie_ReturnsNoContent()
        {
            // Arrange
            int movieId = 1;
            var request = new UpdateMovieRequest { Title = "E" };
            var dto = new UpdateMovieDto { Title = "E" };

            _mapperMock.Setup(m => m.Map<UpdateMovieDto>(request)).Returns(dto);
            _movieServiceMock.Setup(s => s.UpdateMovieAsync(movieId, dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateMovie(movieId, request);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteMovie_ReturnsNoContent()
        {
            // Arrange
            int movieId = 1;
            _movieServiceMock.Setup(s => s.DeleteMovieAsync(movieId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteMovie(movieId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

    }
}
