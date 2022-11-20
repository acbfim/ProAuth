using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ProAuth.WebAPI.Dtos;
using ProAuth.WebAPI.Models.Identity;

namespace ProSales.API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Role, CreateRoleDto>().ReverseMap();
        }
    }
}