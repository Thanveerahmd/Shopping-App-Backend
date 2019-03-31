using AutoMapper;
using Project.Dtos;
using Project.Entities;



namespace  Project.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<Admin, AdminDto>();
            CreateMap<AdminDto, Admin>();
        }
    }
}