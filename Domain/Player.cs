using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Domain;

public class Player
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    public string Name { get; set; }

    [Required] public string SessionToken { get; set; } = $"SESSION_{Guid.NewGuid():N}";

    public Room Room;
    
    public Player(string name)
    {
        Name = name;
    }
}