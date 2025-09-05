using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class BetData
{
    public string playerName;
    public int amount;
    public Team team;
}

public class BettingUIManager : MonoBehaviour
{
 [Header("References (UI)")]
    public RectTransform leftContainer;
    public RectTransform rightContainer;
    public GameObject betIconPrefab;
    public TextMeshProUGUI potText;
    
    [Header("Sprites")]
    public Sprite defaultIcon;
    public Sprite winIcon;
    public Sprite loseIcon;

    [Header("Layout")]
    public float iconSpacing = -16f;
    public float iconVerticalOffset = 0f;
    public float iconScale = 1f;

    [Header("Rotation / Leader animation")]
    public float leaderRotationAmplitude = 12f;
    public float leaderRotationSpeed = 2f;
    public float rotationSmooth = 8f;

    [Header("Debug")]
    [SerializeField]
    private bool debug = false;

    // internal
    Dictionary<string, GameObject> _icons = new Dictionary<string, GameObject>();
    List<BetData> _bets = new List<BetData>();
    Team? _currentLeaderTeam = null;
    int _totalPot = 0;

    void Start()
    {
        if (debug)
        {
            for (int i = 0; i < 5; i++)
            {
                BetData bet = new BetData();
                bet.playerName = $"Player_{i}";
                bet.amount = 1000;
                bet.team = (i % 2 == 0) ? Team.Team1 : Team.Team2;

                AddBet(bet);
            }
            
            UpdateLeaderTeam(Team.Team1);
        }
        
        
        foreach (var bet in BetManager.Instance.GetBets(Team.Team1))
        {
            BetData newBet = new BetData();
            newBet.playerName = bet.playerName;
            newBet.amount = bet.amount;
            
            bet.team = Team.Team1;
            AddBet(newBet);
        }
        
        foreach (var bet in BetManager.Instance.GetBets(Team.Team2))
        {
            BetData newBet = new BetData();
            newBet.playerName = bet.playerName;
            newBet.amount = bet.amount;
            
            bet.team = Team.Team2;
            AddBet(newBet);
        }
        
        RefreshUI();
    }

        void Update()
    {
        if (_icons.Count == 0) return;

        float time = Time.time;
        float angleSin = Mathf.Sin(time * Mathf.PI * 2f * leaderRotationSpeed); // -1..1

        foreach (var kv in _icons)
        {
            GameObject go = kv.Value;
            if (go == null) continue;

            var bet = _bets.Find(b => b.playerName == kv.Key);
            if (bet == null) continue;

            float targetAngle = 0f;

            // === Rotation pour l'équipe gagnante ===
            if (_currentLeaderTeam.HasValue && bet.team == _currentLeaderTeam.Value)
            {
                targetAngle = angleSin * leaderRotationAmplitude;
            }

            Vector3 e = go.transform.localEulerAngles;
            float currentZ = e.z;
            if (currentZ > 180f) currentZ -= 360f;

            float t = 1f - Mathf.Exp(-rotationSmooth * Time.deltaTime);
            float newZ = Mathf.Lerp(currentZ, targetAngle, t);
            go.transform.localEulerAngles = new Vector3(e.x, e.y, newZ);

            // === Changement de sprite (Win/Lose) ===
            var ui = go.GetComponentInChildren<BetIconUI>();
            if (ui != null)
            {
                if (_currentLeaderTeam.HasValue)
                {
                    if (bet.team == _currentLeaderTeam.Value)
                        ui.SetAvatar(winIcon);   // Team gagnante
                    else
                        ui.SetAvatar(loseIcon);  // Team perdante
                }
                else
                {
                    ui.SetAvatar(defaultIcon);   // Pas encore de leader
                }
            }
        }
    }

    public void AddBet(BetData bet)
    {
        _bets.Add(bet);
        _totalPot += bet.amount;
        RefreshUI();
    }

    public void RemoveBet(string playerName)
    {
        var idx = _bets.FindIndex(x => x.playerName == playerName);
        if (idx >= 0)
        {
            _totalPot -= _bets[idx].amount;
            _bets.RemoveAt(idx);
            RefreshUI();
        }
    }

    public void SetPot(int pot)
    {
        _totalPot = pot;
        if (potText != null) potText.text = $"Bets: {_totalPot}";
    }

    public void UpdateLeaderTeam(Team? team)
    {
        _currentLeaderTeam = team;
    }

    public void RefreshUI()
    {
        foreach (var kv in _icons)
        {
            if (kv.Value) Destroy(kv.Value);
        }
        _icons.Clear();

        List<BetData> left = _bets.FindAll(b => b.team == Team.Team1);
        List<BetData> right = _bets.FindAll(b => b.team == Team.Team2);

        for (int i = 0; i < left.Count; i++)
        {
            var go = CreateIcon(left[i], Team.Team1, i);
            _icons[left[i].playerName] = go;
        }

        for (int i = 0; i < right.Count; i++)
        {
            var go = CreateIcon(right[i], Team.Team2, i);
            _icons[right[i].playerName] = go;
        }

        if (potText != null) potText.text = $"Bets: {_totalPot}";
    }

    GameObject CreateIcon(BetData b, Team team, int index)
    {
        var parent = (team == Team.Team1) ? leftContainer : rightContainer;
        var go = Instantiate(betIconPrefab, parent);
        go.name = $"BetIcon_{b.playerName}";
        var rt = go.GetComponent<RectTransform>();
        rt.localScale = Vector3.one * iconScale;

        // Position
        float iconWidth = rt.rect.width * iconScale;
        float spacing = iconSpacing;

        if (team == Team.Team1)
            rt.anchoredPosition = new Vector2(index * (iconWidth + spacing), -iconVerticalOffset);
        else
            rt.anchoredPosition = new Vector2(-index * (iconWidth + spacing), -iconVerticalOffset);

        // Set visuals
        var ui = go.GetComponentInChildren<BetIconUI>();
        if (ui != null)
        {
            ui.SetName(b.playerName);
            ui.SetAmount(b.amount);

            if (defaultIcon != null)
                ui.SetAvatar(defaultIcon);
        }

        go.transform.localEulerAngles = Vector3.zero;
        return go;
    }
}
