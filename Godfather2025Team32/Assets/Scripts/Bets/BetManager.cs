using System.Collections.Generic;
using UnityEngine;

public class BetManager : MonoBehaviour
{
    public static BetManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private List<Bet> bets = new List<Bet>();

    public IReadOnlyList<Bet> Bets => bets;

    // Now accepts Team
    public bool AddBet(string playerName, int amount, Team team)
    {
        if (string.IsNullOrWhiteSpace(playerName))
            return false;
        
        if (amount <= 0) 
            return false;
        
        if (bets.Exists(b => b.playerName == playerName)) 
            return false;

        bets.Add(new Bet(playerName, amount, team));
        return true;
    }

    public bool RemoveBet(string playerName)
    {
        var b = bets.Find(x => x.playerName == playerName);
        if (b == null) 
            return false;
        
        bets.Remove(b);
        return true;
    }

    public void ClearBets()
    {
        bets.Clear();
    }

    public int GetTotalPot()
    {
        int sum = 0;
        foreach (var b in bets) 
            sum += b.amount;
        
        return sum;
    }

    public int ResolveWinner(string winnerName)
    {
        int total = GetTotalPot();
        ClearBets();
        
        return total;
    }
}