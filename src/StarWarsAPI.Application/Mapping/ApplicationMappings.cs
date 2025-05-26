using AutoMapper;
using StarWarsAPI.Application.DTOs;
using StarWarsAPI.Domain.Entities;

namespace StarWarsAPI.Application.Mapping
{
    public class ApplicationMappings : Profile
    {
        public ApplicationMappings()
        {
            CreateMap<RegisterUserDto, User>();
            CreateMap<Movie, MovieResponseDto>();
            CreateMap<CreateMovieDto, Movie>();
        }
    }
}
