using API.Entities;
using API.Extensions;
using API.Models;
using AutoMapper;
using System.Linq;

namespace API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterModel, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username.ToUpper()))
                .ForMember(dest => dest.KnownAs, opt => opt.MapFrom(src => src.KnownAs.ToUpper()))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToUpper()))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City.ToUpper()))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country.ToUpper()));

            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url));

            CreateMap<Photo, PhotoDto>();

            CreateMap<MemberUpdateDto, AppUser>();
        }
    }
}