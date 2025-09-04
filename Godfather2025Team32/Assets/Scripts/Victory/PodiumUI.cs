using TMPro;
using UnityEngine;

public class PodiumUI : MonoBehaviour
{
    private const string MAIN_MENU_SCENE_NAME = "MainMenu";

    public TextMeshProUGUI winnerText;

    void Start()
    {
        GameManager.Instance.HasGameEnded = true;
        Team winnerId = GameManager.Instance.WinnerTeam;
        winnerText.text = "Le gagnant est le joueur " + winnerId + " 🎉";
    }

    public void BackToMainMenu()
    {
        SceneTransitionUI.Instance.LoadSceneWithTransition(MAIN_MENU_SCENE_NAME);
    }
}
