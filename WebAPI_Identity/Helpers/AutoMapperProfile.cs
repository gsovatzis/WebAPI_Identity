using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI_Identity.DTOs;
using WebAPI_Identity.Models;

namespace WebAPI_Identity.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<MyUser, UserDTO>();
            CreateMap<UserDTO, MyUser>();
        }
    }
}
