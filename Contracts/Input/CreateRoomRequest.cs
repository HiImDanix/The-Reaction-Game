using System.ComponentModel.DataAnnotations;

namespace Contracts.Input;

public class CreateRoomRequest
{
    public string PlayerName { get; set; }
}