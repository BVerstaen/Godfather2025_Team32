using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PodiumUI : MonoBehaviour
{
    private const string MAIN_MENU_SCENE_NAME = "MainMenu";

    [Header("UI")]
    public TextMeshProUGUI winnerText;
    public GameObject winnerPanel;
    public RectTransform winnerPanelContent;

    [Header("Prefabs")]
    public GameObject winnerPlayerPrefab;

    [Header("Victory Images")]
    [SerializeField] private MeshRenderer _winnerPanel;
    [SerializeField] private Material _player1Material;
    [SerializeField] private Material _player2Material;

    void Start()
    {
        GameManager.Instance.HasGameEnded = true;
        Team winnerId = GameManager.Instance.WinnerTeam;
        winnerText.text = "Le gagnant est l'équipe " + winnerId + " 🎉";

        if (GameManager.Instance.HasGameEnded)
        {
            winnerPanel.SetActive(true);

            foreach (Transform child in winnerPanelContent)
                Destroy(child.gameObject);

            Dictionary<string, int> payouts = BetManager.Instance.DistributeWinnings(winnerId);

            foreach (var kvp in payouts)
            {
                SpawnWinnerUI(kvp.Key, kvp.Value);
            }
        }

        _winnerPanel.material = GameManager.Instance.WinnerTeam == Team.Team1 ? _player1Material : _player2Material;
    }

    private void SpawnWinnerUI(string playerName, int payout)
    {
        if (winnerPlayerPrefab == null || winnerPanelContent == null)
        {
            Debug.LogWarning("⚠️ Winner prefab ou panel non assigné !");
            return;
        }

        GameObject obj = Instantiate(winnerPlayerPrefab, winnerPanelContent);
        WinnerBetsUI ui = obj.GetComponent<WinnerBetsUI>();

        if (ui)
        {
            ui.Setup(playerName, payout);
        }
        else
        {
            Debug.LogWarning("⚠️ Le prefab du gagnant n'a pas de script WinnerBetsUI !");
        }
    }

    public void BackToMainMenu()
    {
        SceneTransitionUI.Instance.LoadSceneWithTransition(MAIN_MENU_SCENE_NAME);
    }
}
