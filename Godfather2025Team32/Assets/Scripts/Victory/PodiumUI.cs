using TMPro;
using UnityEngine;

public class PodiumUI : MonoBehaviour
{
    public TextMeshProUGUI winnerText;

    void Start()
    {
        Team winnerId = GameManager.Instance.WinnerTeam;
        winnerText.text = "Le gagnant est le joueur " + winnerId + " 🎉";
    }
}
