using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarWarsAPI.Application.Interfaces.IServices;
using System;
using System.Threading.Tasks;

namespace StarWarsAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpPost("sync")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> SyncMovies()
        {
            try
            {
                await _movieService.SyncMoviesAsync();
                return Ok(new { Message = $"Se sincronizaron las películas de Star Wars." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Error al sincronizar películas", Details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMovies()
        {
            var movies = await _movieService.GetAllMoviesAsync();
            return Ok(movies);
        }


    }
}
