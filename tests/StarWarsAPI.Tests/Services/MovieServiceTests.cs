using AutoMapper;
using Moq;
using Moq.Protected;
using StarWarsAPI.Application.DTOs;
using StarWarsAPI.Application.DTOs.Swapi;
using StarWarsAPI.Application.Exceptions;
using StarWarsAPI.Application.Interfaces.IRepositories;
using StarWarsAPI.Application.Services;
using StarWarsAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StarWarsAPI.Tests.Services
{
    public class MovieServiceTests
    {
        private readonly Mock<IMovieRepository> _movieRepository;
        private readonly MovieService _movieService;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<HttpMessageHandler> _httpMessageHandler;
        private readonly HttpClient _httpClient;

        public MovieServiceTests()
        {
            _movieRepository = new Mock<IMovieRepository>();
            _httpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandler.Object);
            _mapper = new Mock<IMapper>();
            _movieService = new MovieService(_httpClient, _movieRepository.Object, _mapper.Object);
        }

        #region DELETE
        [Fact]
        public async Task DeleteMovieAsync_Should_Delete_Existing_Movie()
        {
            // Arrange
            var movieId = 1;
            var movie = new Movie { Id = movieId, Title = "A New Hope" };

            _movieRepository.Setup(r => r.GetByIdAsync(movieId))
                .ReturnsAsync(movie);

            // Act
            await _movieService.DeleteMovieAsync(movieId);

            // Assert
            _movieRepository.Verify(r => r.Delete(movie), Times.Once);
            _movieRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteMovieAsync_Should_Throw_NotFoundException_If_Movie_Does_Not_Exist()
        {
            // Arrange
            var movieId = 1;

            _movieRepository.Setup(r => r.GetByIdAsync(movieId))
                .ReturnsAsync((Movie)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _movieService.DeleteMovieAsync(movieId));
        }

        #endregion

        #region UPDATE

        [Fact]
        public async Task UpdateMovieAsync_Should_Update_Existing_Movie()
        {
            // Arrange
            var movieId = 1;
            var dto = new UpdateMovieDto
            {
                Title = "a",
                EpisodeId = 5,
                OpeningCrawl = "a",
                Director = "a",
                Producer = "a",
                ReleaseDate = new DateTime(1980, 5, 21)
            };

            var movie = new Movie
            {
                Id = movieId,
                Title = "a a",
                EpisodeId = 4,
                OpeningCrawl = "a",
                Director = "a a",
                Producer = "a a",
                ReleaseDate = new DateTime(1977, 5, 25)
            };

            _movieRepository.Setup(r => r.GetByIdAsync(movieId))
                .ReturnsAsync(movie);

            // Act
            await _movieService.UpdateMovieAsync(movieId, dto);

            // Assert
            Assert.Equal(dto.Title, movie.Title);
            Assert.Equal(dto.EpisodeId, movie.EpisodeId);
            Assert.Equal(dto.OpeningCrawl, movie.OpeningCrawl);
            Assert.Equal(dto.Director, movie.Director);
            Assert.Equal(dto.Producer, movie.Producer);
            Assert.Equal(dto.ReleaseDate, movie.ReleaseDate);

            _movieRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateMovieAsync_Should_Throw_NotFoundException_If_Movie_Does_Not_Exist()
        {
            // Arrange
            var movieId = 1;
            var dto = new UpdateMovieDto
            {
                Title = "b",
                EpisodeId = 5,
                OpeningCrawl = "b",
                Director = "b",
                Producer = "b",
                ReleaseDate = new DateTime(1980, 5, 21)
            };

            _movieRepository.Setup(r => r.GetByIdAsync(movieId))
                .ReturnsAsync((Movie)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _movieService.UpdateMovieAsync(movieId, dto));
        }

        #endregion

        #region CREATE

        [Fact]
        public async Task CreateMovieAsync_Should_Create_And_Return_MovieResponseDto()
        {
            // Arrange
            var dto = new CreateMovieDto
            {
                Title = "c",
                Director = "c c",
                Producer = "c Gc c",
                ReleaseDate = new DateTime(1983, 5, 25),
                EpisodeId = 6,
                OpeningCrawl = "c"
            };

            var movie = new Movie
            {
                Title = dto.Title,
                Director = dto.Director,
                Producer = dto.Producer,
                ReleaseDate = dto.ReleaseDate,
                EpisodeId = dto.EpisodeId,
                OpeningCrawl = dto.OpeningCrawl
            };

            var expectedResponse = new MovieResponseDto
            {
                Title = dto.Title,
                Director = dto.Director,
                Producer = dto.Producer,
                ReleaseDate = dto.ReleaseDate,
                OpeningCrawl = dto.OpeningCrawl
            };

            _mapper.Setup(m => m.Map<Movie>(dto)).Returns(movie);
            _movieRepository.Setup(r => r.AddAsync(movie)).Returns(Task.CompletedTask);
            _movieRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapper.Setup(m => m.Map<MovieResponseDto>(movie)).Returns(expectedResponse);

            // Act
            var result = await _movieService.CreateMovieAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Title, result.Title);
            Assert.Equal(expectedResponse.Director, result.Director);
            Assert.Equal(expectedResponse.Producer, result.Producer);
            Assert.Equal(expectedResponse.ReleaseDate, result.ReleaseDate);
            Assert.Equal(expectedResponse.OpeningCrawl, result.OpeningCrawl);

            _movieRepository.Verify(r => r.AddAsync(movie), Times.Once);
            _movieRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateMovieAsync_Should_Call_AddAsync_And_SaveChangesAsync()
        {
            // Arrange
            var dto = new CreateMovieDto
            {
                Title = "d",
                Director = "d",
                Producer = "d",
                ReleaseDate = new DateTime(1983, 5, 25),
                EpisodeId = 6,
                OpeningCrawl = "d"
            };

            var movie = new Movie();

            _mapper.Setup(m => m.Map<Movie>(dto)).Returns(movie);
            _movieRepository.Setup(r => r.AddAsync(movie)).Returns(Task.CompletedTask);
            _movieRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapper.Setup(m => m.Map<MovieResponseDto>(movie)).Returns(new MovieResponseDto());

            // Act
            await _movieService.CreateMovieAsync(dto);

            // Assert
            _movieRepository.Verify(r => r.AddAsync(movie), Times.Once);
            _movieRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region GET BY ID

        [Fact]
        public async Task GetMovieByIdAsync_Should_Return_MovieResponseDto_When_Movie_Exists()
        {
            // Arrange
            var movieId = 1;
            var movie = new Movie
            {
                Id = movieId,
                Title = "Ad",
                Director = "Gf",
                Producer = "f",
                ReleaseDate = new DateTime(1977, 5, 25),
                EpisodeId = 4,
                OpeningCrawl = "f."
            };

            var responseDto = new MovieResponseDto
            {
                Title = movie.Title,
                Director = movie.Director,
                Producer = movie.Producer,
                ReleaseDate = movie.ReleaseDate,
                OpeningCrawl = movie.OpeningCrawl
            };

            _movieRepository.Setup(r => r.GetByIdAsync(movieId)).ReturnsAsync(movie);
            _mapper.Setup(m => m.Map<MovieResponseDto>(movie)).Returns(responseDto);

            // Act
            var result = await _movieService.GetMovieByIdAsync(movieId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(movie.Title, result.Title);
            Assert.Equal(movie.Director, result.Director);
            Assert.Equal(movie.Producer, result.Producer);
            Assert.Equal(movie.ReleaseDate, result.ReleaseDate);
            Assert.Equal(movie.OpeningCrawl, result.OpeningCrawl);
        }

        [Fact]
        public async Task GetMovieByIdAsync_Should_Return_Null_When_Movie_Does_Not_Exist()
        {
            // Arrange
            var movieId = 99;
            _movieRepository.Setup(r => r.GetByIdAsync(movieId)).ReturnsAsync((Movie)null);

            // Act
            var result = await _movieService.GetMovieByIdAsync(movieId);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GET ALL

        [Fact]
        public async Task GetAllMoviesAsync_Should_Return_List_Of_MovieResponseDto()
        {
            // Arrange
            var movies = new List<Movie>
            {
                new Movie
                {
                    Id = 1,
                    Title = "a",
                    Director = "a",
                    Producer = "a",
                    ReleaseDate = new DateTime(1977, 5, 25),
                    EpisodeId = 4,
                    OpeningCrawl = "a"
                },
                new Movie
                {
                    Id = 2,
                    Title = "a",
                    Director = "a",
                    Producer = "a",
                    ReleaseDate = new DateTime(1980, 5, 21),
                    EpisodeId = 5,
                    OpeningCrawl = "a"
                }
            };

            var responseDtos = new List<MovieResponseDto>
            {
                new MovieResponseDto
                {
                    Title = "a",
                    Director = "a",
                    Producer = "a",
                    ReleaseDate = new DateTime(1977, 5, 25),
                    OpeningCrawl = "a"
                },
                new MovieResponseDto
                {
                    Title = "a",
                    Director = "a",
                    Producer = "a",
                    ReleaseDate = new DateTime(1980, 5, 21),
                    OpeningCrawl = "a"
                }
            };

            _movieRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(movies);
            _mapper.Setup(m => m.Map<List<MovieResponseDto>>(movies)).Returns(responseDtos);

            // Act
            var result = await _movieService.GetAllMoviesAsync();

            // Assert
            Assert.NotNull(result);
            var list = Assert.IsAssignableFrom<IEnumerable<MovieResponseDto>>(result);
            Assert.Equal(movies.Count, new List<MovieResponseDto>(list).Count);

            var resultList = new List<MovieResponseDto>(list);
            for (int i = 0; i < movies.Count; i++)
            {
                Assert.Equal(movies[i].Title, resultList[i].Title);
                Assert.Equal(movies[i].Director, resultList[i].Director);
                Assert.Equal(movies[i].Producer, resultList[i].Producer);
                Assert.Equal(movies[i].ReleaseDate, resultList[i].ReleaseDate);
                Assert.Equal(movies[i].OpeningCrawl, resultList[i].OpeningCrawl);
            }
        }

        [Fact]
        public async Task GetAllMoviesAsync_Should_Return_Empty_List_When_No_Movies()
        {
            // Arrange
            var movies = new List<Movie>();
            var responseDtos = new List<MovieResponseDto>();

            _movieRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(movies);
            _mapper.Setup(m => m.Map<List<MovieResponseDto>>(movies)).Returns(responseDtos);

            // Act
            var result = await _movieService.GetAllMoviesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region SYNC
        [Fact]
        public async Task SyncMoviesAsync_Should_AddNewMovies_When_ThereAreNewOnes()
        {
            // Arrange
            var swapiResponse = new SwapiFilmListResponseDto
            {
                Result = new List<SwapiFilmItemDto>
                {
                    new SwapiFilmItemDto
                    {
                        Properties = new SwapiFilmPropertiesDto
                        {
                            EpisodeId = 4,
                            Title = "a",
                            OpeningCrawl = "a",
                            Director = "Ga",
                            Producer = "a",
                            ReleaseDate = new DateTime(1977, 5, 25)
                        }
                    },
                    new SwapiFilmItemDto
                    {
                        Properties = new SwapiFilmPropertiesDto
                        {
                            EpisodeId = 5,
                            Title = "a",
                            OpeningCrawl = "a",
                            Director = "a",
                            Producer = "a",
                            ReleaseDate = new DateTime(1980, 5, 21)
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(swapiResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            });

            _httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            _movieRepository.Setup(r => r.GetExistingEpisodeIdsAsync())
                .ReturnsAsync(new List<int> { 4 }); // Ya existe episodio 4

            _movieRepository.Setup(r => r.AddRangeAsync(It.IsAny<List<Movie>>()))
                .Returns(Task.CompletedTask);

            _movieRepository.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _movieService.SyncMoviesAsync();

            // Assert
            // Solo debería agregar la película con EpisodeId 5
            _movieRepository.Verify(r => r.AddRangeAsync(
                It.Is<List<Movie>>(list => list.Count == 1 && list[0].EpisodeId == 5)), Times.Once);
            _movieRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task SyncMoviesAsync_Should_NotAddMovies_When_AllExist()
        {
            // Arrange
            var swapiResponse = new SwapiFilmListResponseDto
            {
                Result = new List<SwapiFilmItemDto>
                {
                    new SwapiFilmItemDto
                    {
                        Properties = new SwapiFilmPropertiesDto
                        {
                            EpisodeId = 4,
                            Title = "a"
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(swapiResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            });

            _httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            _movieRepository.Setup(r => r.GetExistingEpisodeIdsAsync())
                .ReturnsAsync(new List<int> { 4 });

            // Act
            await _movieService.SyncMoviesAsync();

            // Assert
            _movieRepository.Verify(r => r.AddRangeAsync(It.IsAny<List<Movie>>()), Times.Never);
            _movieRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task SyncMoviesAsync_Should_Return_When_ResponseIsEmptyOrNull()
        {
            // Arrange - Empty result
            var swapiResponse = new SwapiFilmListResponseDto
            {
                Result = new List<SwapiFilmItemDto>()
            };

            var json = JsonSerializer.Serialize(swapiResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            });

            _httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            // Act
            await _movieService.SyncMoviesAsync();

            // Assert
            _movieRepository.Verify(r => r.AddRangeAsync(It.IsAny<List<Movie>>()), Times.Never);
            _movieRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task SyncMoviesAsync_Should_Throw_When_HttpResponse_IsNotSuccess()
        {
            // Arrange
            _httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _movieService.SyncMoviesAsync());
        }
        #endregion
    }
}
