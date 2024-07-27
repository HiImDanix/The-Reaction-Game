﻿using System.ComponentModel.DataAnnotations;
using System.Text;
using Domain.Constants;

namespace Domain;

public class Room
{
    private static readonly Random Random = new Random();
    
    public enum RoomStatus
    {
        Lobby,
        Starting,
        InProgress,
        Finished
    }
    
    [Key]
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    [Required]
    [MaxLength(RoomConstants.MaxCodeLength)]
    public string Code { get; private set; }
    public RoomStatus Status { get; set; }
    [Range(1, RoomConstants.MaxPlayers)]
    public List<Player> Players { get; private set; }  = new();
    [Required]
    public Player Host { get; set; }
    public List<Game> Games { get; private set; } = new();
    public Game? CurrentGame { get; set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    

    public Room(Player host)
    {
        Code = GenerateRoomCode();
        Status = RoomStatus.Lobby;
        Host = host;
        Players.Add(host);
    }
    
    public Room()
    {}

    private static string GenerateRoomCode()
    {
       var generatedCode = new StringBuilder(RoomConstants.MaxCodeLength);
       for (var i = 0; i < RoomConstants.MaxCodeLength; i++)
       {
           generatedCode.Append(RoomConstants.CodeChars[Random.Next(RoomConstants.CodeChars.Length)]);
       }
       
       return generatedCode.ToString();
    }

}