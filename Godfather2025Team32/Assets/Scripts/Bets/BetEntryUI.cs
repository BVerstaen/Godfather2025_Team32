using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BetEntryUI : MonoBehaviour
{
    [Header("Text (use either one)")]
    public TextMeshProUGUI tmpMainText;
    public Button buttonRemove;

    private string playerName;
    private Action<string> onRemove;

    public void Setup(string playerName, int amount, Team team, Action<string> onRemove)
    {
        this.playerName = playerName;
        this.onRemove = onRemove;

        string combined = $"{playerName} - {amount}€ - {team}";
        if (tmpMainText != null) tmpMainText.text = combined;

        if (buttonRemove != null)
        {
            buttonRemove.onClick.RemoveAllListeners();
            buttonRemove.onClick.AddListener(() => onRemove?.Invoke(playerName));
        }
    }
}