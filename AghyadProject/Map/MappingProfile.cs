using AghyadProject.Dtos;
using AghyadProject.Models;
using AutoMapper;

namespace AghyadProject.Map
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserRegisterDto>();
        }
    }
}
