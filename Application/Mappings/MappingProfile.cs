using AutoMapper;
using Domain;
using Contracts.Output;

namespace Application.Mappings;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<Room, RoomOut>();
        CreateMap<(Room, Player), RoomOut>()
            .ForMember(dest => dest.You, opt => opt.MapFrom(src => src.Item2))
            .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.Item1.Players))
            .ForMember(dest => dest.Host, opt => opt.MapFrom(src => src.Item1.Host))
            .ForMember(dest => dest.Games, opt => opt.MapFrom(src => src.Item1.Games))
            .ForMember(dest => dest.CurrentGame, opt => opt.MapFrom(src => src.Item1.CurrentGame))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Item1.CreatedAt))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Item1.Status))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Item1.Code))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Item1.Id));
        CreateMap<Player, PlayerOut>();
        CreateMap<Player, PlayerPersonalOut>();
        CreateMap<Game, GameOut>();
    }
}