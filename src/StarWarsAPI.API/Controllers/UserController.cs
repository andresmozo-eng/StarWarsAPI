using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StarWarsAPI.API.Models.Requests;
using StarWarsAPI.Application.DTOs;
using StarWarsAPI.Application.Interfaces.IServices;
using System.Threading.Tasks;

namespace StarWarsAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="request">Los datos necesarios para registrar un usuario.</param>
        /// <response code="200">Usuario registrado con éxito.</response>
        /// <response code="400">Los datos proporcionados no son válidos.</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {
            await _userService.RegisterUserAsync(_mapper.Map<RegisterUserDto>(request));
            return Ok(new { Message = "Usuario registrado con éxito" });
        }

        /// <summary>
        /// Inicia sesión un usuario existente.
        /// </summary>
        /// <param name="request">Credenciales del usuario.</param>
        /// <returns>Token JWT si las credenciales son válidas.</returns>
        /// <response code="200">Inicio de sesión exitoso, devuelve el token.</response>
        /// <response code="400">Credenciales inválidas.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserRequest request)
        {
            var token = await _userService.LoginUserAsync(_mapper.Map<LoginUserDto>(request));
            return Ok(new { Token = token });
        }
    }
}
