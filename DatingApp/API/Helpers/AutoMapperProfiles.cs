using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDTO>()
            .ForMember(des=> des.PhotoUrl,
             opt=> opt.MapFrom(src=> src.Photos.FirstOrDefault(s=>s.IsMain).Url));
            CreateMap<Photo, PhotoDTO>();
        }
    }
}