using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryZone : MonoBehaviour
{
    [Tooltip("Nom de la scène du podium")]
    public string podiumSceneName = "PodiumScene";

    private int _playerFinish = 0;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            _playerFinish++;

            if (_playerFinish == 1)
                GameManager.Instance.SetWinner(player.currentTeam);
            
            if (_playerFinish == 1)
                SceneTransitionUI.Instance.LoadSceneWithTransition(podiumSceneName);
            
            //Destroy(other.gameObject);
        }
    }
}