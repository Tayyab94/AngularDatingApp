using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDTO>()
            .ForMember(des=> des.PhotoUrl,
             opt=> opt.MapFrom(src=> src.Photos.FirstOrDefault(s=>s.IsMain).Url))
             .ForMember(des=> des.Age, opt=> opt.MapFrom(src=>src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDTO>();
            CreateMap<PhotoDTO, Photo>();

            CreateMap<MemberUpdateDTO, AppUser>();

            CreateMap<RegistrationDTO, AppUser>();

            CreateMap<Message,MessageDTO>()
            .ForMember(s=>s.SenderPhotoUrl,opt=> opt.MapFrom(src=>
                src.Sender.Photos.FirstOrDefault(s=>s.IsMain).Url))
            .ForMember(s=>s.ReceiverPhotoUrl, opt=> opt.MapFrom(sr=>
            sr.Receiver.Photos.FirstOrDefault(s=>s.IsMain).Url));
        }
    }
}