using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryZone : MonoBehaviour
{
    [SerializeField] private int _numberOfPlayers = 1;

    private int _playerFinish = 0;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            _playerFinish++;

            if (_playerFinish == _numberOfPlayers)
                GameManager.Instance.SetRoundWinner(player.currentTeam);
        }
    }
}