using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Game
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
}