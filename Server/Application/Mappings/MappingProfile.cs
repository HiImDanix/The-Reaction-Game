using AutoMapper;
using Domain;
using Contracts.Output;
using Contracts.Output.Hub;

namespace Application.Mappings;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<Room, RoomResp>();
        CreateMap<(Room, Player, string), RoomCreatedPersonalResp>()
            .ForMember(dto => dto.Room, opt => opt.MapFrom(tuple => tuple.Item1))
            .ForMember(dto => dto.You, opt => opt.MapFrom(tuple => tuple.Item2))
            .ForMember(dto => dto.SessionToken, opt => opt.MapFrom(tuple => tuple.Item3));
        CreateMap<(Room, Player, string), RoomJoinedPersonalResp>()
            .ForMember(dto => dto.Room, opt => opt.MapFrom(tuple => tuple.Item1))
            .ForMember(dto => dto.You, opt => opt.MapFrom(tuple => tuple.Item2))
            .ForMember(dto => dto.SessionToken, opt => opt.MapFrom(tuple => tuple.Item3));
        CreateMap<Player, PlayerResp>();
        CreateMap<Game, GameResp>();
        CreateMap<Player, PlayerJoinedMessage>()
            .ForMember(dto => dto.Player, opt => opt.MapFrom(player => player));
    }
}