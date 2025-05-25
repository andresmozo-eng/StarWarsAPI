using AutoMapper;
using StarWarsAPI.Application.DTOs;
using StarWarsAPI.Domain.Entities;

namespace StarWarsAPI.Application.Mapping
{
    public class DtoToEntityProfile : Profile
    {
        public DtoToEntityProfile()
        {
            CreateMap<RegisterUserDto, User>();
        }
    }
}
