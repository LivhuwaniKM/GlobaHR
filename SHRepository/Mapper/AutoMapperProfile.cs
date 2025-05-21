using AutoMapper;
using SHDomain.Models.User;

namespace SHServices.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}
