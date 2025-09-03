using System.Collections.Generic;
using UnityEngine;
using static ButtonsInputs;
using static CircularMovementDetector;

[CreateAssetMenu(fileName = "SequenceSO", menuName = "Scriptable Objects/SequenceSO")]
public class SequenceSO : ScriptableObject
{
    public enum SequenceDifficulty
    {
        Easy,
        Normal,
        Hard,
        ChadHard
    }


    [System.Serializable]
    public struct Sequence
    {
        public List<Buttons> ButtonsSequences;
        [Tooltip("Le nombre de fois qu'il faut répéter cette sequence de touches")]
        public int NumberOfRepeats;
    }

    public SequenceDifficulty Difficulty;

    public List<Sequence> ButtonSequenceList;

    public RotationDirection LeftGigaChadRotation;
    public RotationDirection RightGigaChadRotation;
}
