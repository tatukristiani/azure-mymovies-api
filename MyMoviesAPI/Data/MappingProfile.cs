using AutoMapper;
using MyMoviesAPI.Data.Entities;
using MyMoviesAPI.Models.User;

namespace MyMoviesAPI.Data
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>()
                .ForMember(u => u.ID, ex => ex.MapFrom(i => i.ID));

            CreateMap<UserCreateDTO, User>()
                .ForMember(u => u.Email, ex => ex.MapFrom(i => i.Email));
        }
    }
}
