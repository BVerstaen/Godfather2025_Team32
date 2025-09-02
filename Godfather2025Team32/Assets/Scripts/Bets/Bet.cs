[System.Serializable]
public class Bet
{
    public string playerName;
    public int amount;
    public Team team;

    public Bet(string playerName, int amount, Team team)
    {
        this.playerName = playerName;
        this.amount = amount;
        this.team = team;
    }
}