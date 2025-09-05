using UnityEngine;

public class VictoryZone : MonoBehaviour
{
    [SerializeField] private int _numberOfPlayers = 1;

    private int _playerFinish = 0;

    private PlayerController firstPlayer;
    private void OnTriggerEnter(Collider other)
    {
        print(other.gameObject.name);
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            if(firstPlayer == null)
                firstPlayer = player;

            _playerFinish++;

            if (_playerFinish == _numberOfPlayers)
                GameManager.Instance.SetRoundWinner(firstPlayer.currentTeam);
        }
    }
}