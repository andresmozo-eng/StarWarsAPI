using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarWarsAPI.API.Models.Requests;
using StarWarsAPI.Application.DTOs;
using StarWarsAPI.Application.Interfaces.IServices;
using StarWarsAPI.Application.Services;
using System;
using System.Threading.Tasks;

namespace StarWarsAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IMapper _mapper;

        public MoviesController(IMovieService movieService, IMapper mapper)
        {
            _movieService = movieService;
            _mapper = mapper;
        }

        /// <summary>
        /// Sincroniza las películas de Star Wars desde la API pública.
        /// </summary>
        /// <remarks>Este endpoint requiere el rol de Administrador.</remarks>
        /// <returns>Retorna un mensaje indicando que la sincronización fue exitosa.</returns>
        /// <response code="200">La sincronización se realizó correctamente.</response>
        /// <response code="401">El usuario no está autenticado.</response>
        /// <response code="403">El usuario no tiene permisos para sincronizar.</response>
        /// <response code="500">Error interno del servidor al sincronizar las películas.</response>
        [HttpPost("sync")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> SyncMovies()
        {
            await _movieService.SyncMoviesAsync();
            return Ok(new { Message = $"Se sincronizaron las películas de Star Wars." });
        }

        /// <summary>
        /// Obtiene la lista de todas las películas almacenadas.
        /// </summary>
        /// <returns>Una lista de películas.</returns>
        /// <response code="200">Devuelve la lista de películas.</response>
        /// <response code="401">El usuario no está autenticado.</response>
        [HttpGet]
        [Authorize] // Opcional: solo si querés que sea solo para usuarios logueados
        public async Task<IActionResult> GetMovies()
        {
            var movies = await _movieService.GetAllMoviesAsync();
            return Ok(movies);
        }


        /// <summary>
        /// Obtiene los detalles de una película por su ID.
        /// </summary>
        /// <param name="id">ID de la película.</param>
        /// <returns>Detalles de la película.</returns>
        /// <response code="200">Devuelve los detalles de la película.</response>
        /// <response code="404">No se encontró una película con el ID especificado.</response>
        /// <response code="401">El usuario no está autenticado.</response>
        /// <response code="403">El usuario no tiene permisos para acceder.</response>
        [HttpGet("{id}")]
       // [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMovieById(int id)
        {
            var movie = await _movieService.GetMovieByIdAsync(id);
            if (movie == null)
                return NotFound(new { Message = $"No se encontró una película con ID {id}." });

            return Ok(movie);
        }

        /// <summary>
        /// Crea una nueva película en la base de datos.
        /// </summary>
        /// <param name="request">Los datos de la película a crear.</param>
        /// <returns>La película creada.</returns>
        /// <response code="201">Película creada exitosamente.</response>
        /// <response code="400">Los datos proporcionados no son válidos.</response>
        /// <response code="401">El usuario no está autenticado.</response>
        /// <response code="403">El usuario no tiene permisos para crear películas.</response>
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateMovie([FromBody] CreateMovieRequest request)
        {
            var movie = await _movieService.CreateMovieAsync(_mapper.Map<CreateMovieDto>(request));
            return Ok(movie);
        }

    }
}
