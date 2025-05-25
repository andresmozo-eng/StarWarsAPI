using AutoMapper;
using StarWarsAPI.API.Models.Requests;
using StarWarsAPI.Application.DTOs;

namespace StarWarsAPI.API.Mapping
{
    public class RequestToDtoProfile : Profile
    {
        public RequestToDtoProfile()
        {
            CreateMap<RegisterUserRequest, RegisterUserDto>();
            CreateMap<LoginUserRequest, LoginUserDto>();
        }
    }
}
