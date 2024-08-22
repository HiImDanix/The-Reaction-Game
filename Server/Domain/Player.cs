using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Domain;

public class Player
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    public string Name { get; set; }
    
    public Player(string name)
    {
        Name = name;
    }
}