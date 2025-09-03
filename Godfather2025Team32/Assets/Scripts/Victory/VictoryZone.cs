using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryZone : MonoBehaviour
{
    [Tooltip("Nom de la scène du podium")]
    public string podiumSceneName = "PodiumScene";

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            GameManager.Instance.SetWinner(player.currentTeam);
            SceneManager.LoadScene(podiumSceneName);
        }
    }
}