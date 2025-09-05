using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MarketItem
{
    public string itemId;
    public int price;
    public Sprite spriteToUnlock;
}

public class MarketManager : MonoBehaviour
{
    public static MarketManager Instance { get; private set; }

    [Header("Money Settings")]
    public int baseIncomePerSecond = 10;
    public bool multiplierActiveTeam1 = false;
    public bool multiplierActiveTeam2 = false;
    public float multiplier = 2f;

    [Header("UI")]
    public TextMeshProUGUI moneyTextTeam1;
    public TextMeshProUGUI moneyTextTeam2;

    private float moneyTeam1 = 0;
    private float moneyTeam2 = 0;
    private float timer = 0f;

    [Header("Market Items")]
    public List<MarketItem> itemTeam1 = new List<MarketItem>();
    public List<MarketItem> itemTeam2 = new List<MarketItem>();
    
    private HashSet<string> unlockedItemsTeam1 = new HashSet<string>();
    private HashSet<string> unlockedItemsTeam2 = new HashSet<string>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        EventManager.Instance.OnStartGigaChad += ToggleMultiplierOn;
        EventManager.Instance.OnEndGigaChad += ToggleMultiplierOff;
    }

    void Update()
    {
        /*timer += Time.deltaTime;
        if (timer >= 1f)
        {
            timer -= 1f;
            GainIncome();
        }*/

        if (!GameManager.Instance.HasGameEnded)
        {
            GetMoneyIncome();
        }

        if (moneyTextTeam1)
            moneyTextTeam1.text = $" {Mathf.RoundToInt(moneyTeam1)}";
        else
            moneyTextTeam1 = GameObject.FindGameObjectWithTag("Money1").GetComponent<TextMeshProUGUI>();
        
        if (moneyTextTeam2)
            moneyTextTeam2.text = $" {Mathf.RoundToInt(moneyTeam2)}";
        else
            moneyTextTeam2 = GameObject.FindGameObjectWithTag("Money2").GetComponent<TextMeshProUGUI>();
    }

    private void GetMoneyIncome()
    {
        float income = baseIncomePerSecond;

        if (multiplierActiveTeam1)
        {
            income *= multiplier;
        }
        moneyTeam1 += income * Time.deltaTime;

        income = baseIncomePerSecond;
        if (multiplierActiveTeam2)
        {
            income *= multiplier;
        }
        moneyTeam2 += income * Time.deltaTime;
    }

    void GainIncome()
    {
        int income = baseIncomePerSecond;

        if (multiplierActiveTeam1)
        {
            income = Mathf.RoundToInt(income * multiplier);
        }
        moneyTeam1 += income;

        income = baseIncomePerSecond;
        if (multiplierActiveTeam2)
        {
            income = Mathf.RoundToInt(income * multiplier);
        }
        moneyTeam2 += income;
    }

    private void ToggleMultiplierOn(Team team, SequenceSO sO) => ToggleMultiplier(true, team);
    private void ToggleMultiplierOff(Team team) => ToggleMultiplier(false, team);
    public void ToggleMultiplier(bool state, Team team)
    {
        if (team == Team.Team1)
            multiplierActiveTeam2 = state;
        else
            multiplierActiveTeam1 = state;
    }

    public void AddMoney(int amount, Team team)
    {
        if (team == Team.Team1)
            moneyTeam1 += amount;
        else
            moneyTeam2 += amount;
    }

    public int GetMoney(Team team)
    {
        return Mathf.RoundToInt((team == Team.Team1) ? moneyTeam1 : moneyTeam2);
    }

    public bool TryUnlock(string itemId, int price, Team team)
    {
        int currentMoney = GetMoney(team);
        HashSet<string> unlockedSet = (team == Team.Team1) ? unlockedItemsTeam1 : unlockedItemsTeam2;

        if (unlockedSet.Contains(itemId))
        {
            Debug.Log($"⚠️ {itemId} déjà débloqué par {team} !");
            return false;
        }

        if (currentMoney >= price)
        {
            AddMoney(-price, team);
            unlockedSet.Add(itemId);

            Debug.Log($"✅ {itemId} débloqué par {team} pour {price} !");
            return true;
        }

        Debug.Log($"❌ Pas assez d'argent pour débloquer {itemId} !");
        return false;
    }

    public bool IsUnlocked(string itemId, Team team)
    {
        return (team == Team.Team1) ? unlockedItemsTeam1.Contains(itemId) : unlockedItemsTeam2.Contains(itemId);
    }

    public Sprite GetRandomUnlockedSprite(Team team)
    {
        HashSet<string> unlockedSet = (team == Team.Team1) ? unlockedItemsTeam1 : unlockedItemsTeam2;
        List<Sprite> unlockedSprites = new List<Sprite>();

        if(team == Team.Team1)
        {
            foreach (var item in itemTeam1)
            {
                if (unlockedSet.Contains(item.itemId) && item.spriteToUnlock != null)
                    unlockedSprites.Add(item.spriteToUnlock);
            }
        }
        else if(team == Team.Team2)
        {
            foreach (var item in itemTeam2)
            {
                if (unlockedSet.Contains(item.itemId) && item.spriteToUnlock != null)
                    unlockedSprites.Add(item.spriteToUnlock);
            }
        }

        if (unlockedSprites.Count == 0)
            return null;

        return unlockedSprites[Random.Range(0, unlockedSprites.Count)];
    }

    public MarketItem findMarketItemFromID(string ID)
    {
        foreach (MarketItem item in itemTeam1)
        {
            if (item.itemId == ID)
                return item;
        }
        foreach (MarketItem item in itemTeam2)
        {
            if (item.itemId == ID)
                return item;
        }

        return null;
    }
}