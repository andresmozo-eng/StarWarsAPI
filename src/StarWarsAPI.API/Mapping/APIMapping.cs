using AutoMapper;
using StarWarsAPI.API.Models.Requests;
using StarWarsAPI.Application.DTOs;

namespace StarWarsAPI.API.Mapping
{
    public class APIMapping : Profile
    {
        public APIMapping()
        {
            CreateMap<RegisterUserRequest, RegisterUserDto>();
            CreateMap<LoginUserRequest, LoginUserDto>();
            CreateMap<CreateMovieRequest, CreateMovieDto>();
            CreateMap<UpdateMovieRequest, UpdateMovieDto>();
        }
    }
}
