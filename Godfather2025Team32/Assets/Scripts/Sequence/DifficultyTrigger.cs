using UnityEngine;
using static SequenceSO;

public class DifficultyTrigger : MonoBehaviour
{
    [SerializeField] private SequenceDifficulty _newDifficulty;

    private void OnTriggerEnter(Collider other)
    {
        SequenceManager foundSequenceManager = other.GetComponent<SequenceManager>();
        if (foundSequenceManager != null)
        {
            foundSequenceManager.CurrentDifficulty = _newDifficulty;
        }
    }
}
