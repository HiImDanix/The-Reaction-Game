using System.ComponentModel.DataAnnotations;

namespace Contracts.Input;

public class CreateRoomReq
{
    public string PlayerName { get; set; }
}