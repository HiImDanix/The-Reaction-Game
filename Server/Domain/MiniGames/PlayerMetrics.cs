namespace Domain.MiniGames;

public class PlayerMetrics
{
    public Player Player { get; set; }
    public MiniGameRound Round { get; set; }
    public double Speed { get; set; }
    public double Accuracy { get; set; }
    public int ResponseOrder { get; set; }
    public bool IsCorrect { get; set; }
    
    public PlayerMetrics(Player player, MiniGameRound round, double speed, double accuracy, int responseOrder)
    {
        Player = player;
        Round = round;
        Speed = speed;
        Accuracy = accuracy;
        ResponseOrder = responseOrder;
    }
    
    public PlayerMetrics()
    {
    }
}