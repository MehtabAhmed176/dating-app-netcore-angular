using API.DTOs;
using api.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers;

public class AutoMappersProfiles:Profile
{
    public AutoMappersProfiles()
    {
        CreateMap<AppUser, MemberDto>()
            .ForMember( dest => dest.PhotoUrl,
                opt => opt.MapFrom(src => 
                    src.Photos.FirstOrDefault(x=> x.IsMain).Url) )
            .ForMember(dest => dest.Age
                , opt => 
                    opt.MapFrom(src => src.DateOfBirth.CalculateAge()) )
            ;
        CreateMap<Photo, PhotoDto>();
        CreateMap<MemberUpdateDto, AppUser>();
        CreateMap<RegisterDto, AppUser>();
    } 
}