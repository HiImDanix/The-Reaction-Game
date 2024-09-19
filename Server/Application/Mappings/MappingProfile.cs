using AutoMapper;
using Domain;
using Contracts.Output;
using Contracts.Output.Hub;
using Contracts.Output.MiniGames;
using Domain.MiniGames;

namespace Application.Mappings;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<Room, RoomResp>()
            .ForMember(dto => dto.Host,
                opt => opt.MapFrom(room => room.Players.Find(player => player.Id == room.HostId)));
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
        CreateMap<MiniGame, MiniGameResp>();
        CreateMap<MiniGameRound, MiniGameRoundResp>()
            .Include<ColorTapRound, ColorTapRoundResp>();
        CreateMap<ColorTapGame, ColorTapGameResp>();
        CreateMap<ColorTapRound, ColorTapRoundResp>();
        CreateMap<ColorTapWordPairDisplay, ColorTapWordPairDisplayResp>();
    }
}