using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers;

public sealed class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDto>()
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.Single(x => x.IsMain).Url))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
        CreateMap<Photo, PhotoDto>();
        CreateMap<MemberUpdateDto, AppUser>();
        CreateMap<RegisterDto, AppUser>();
        CreateMap<Message, MessageDto>()
            .ForMember(x => x.SenderPhotoUrl, x => x.MapFrom(s => s.Sender.Photos.First(a => a.IsMain).Url))
            .ForMember(x => x.RecipientPhotoUrl, x => x.MapFrom(s => s.Recipient.Photos.First(a => a.IsMain).Url));
        CreateMap<DateTime, DateTime>().ConvertUsing(x => DateTime.SpecifyKind(x, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(x => 
            x.HasValue 
                ?  DateTime.SpecifyKind(x.Value, DateTimeKind.Utc) 
                : null);
    }
}