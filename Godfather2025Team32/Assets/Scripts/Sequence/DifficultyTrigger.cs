using UnityEngine;
using static SequenceSO;

public class DifficultyTrigger : MonoBehaviour
{
    [SerializeField] private SequenceDifficulty _newDifficulty;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController foundPlayerController = other.GetComponent<PlayerController>();
        if (foundPlayerController != null)
        {
            EventManager.Instance.ChangeDifficulty(foundPlayerController.currentTeam, _newDifficulty);
        }
    }
}
