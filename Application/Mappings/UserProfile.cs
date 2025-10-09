using AutoMapper;
using SNI_Events.Application.Dtos.User;
using SNI_Events.Domain.Entities;

namespace SNI_Events.Application.Mappings
{
    public class UserProfile : Profile // Extend AutoMapper's Profile class
    {
        public UserProfile() // Provide a constructor with a body
        {
            CreateMap<User, UserDto>().ReverseMap(); // Correctly use AutoMapper's CreateMap method
        }
    }
}
