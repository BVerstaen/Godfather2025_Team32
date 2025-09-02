using System.Collections.Generic;
using UnityEngine;
using static ButtonsInputs;

[CreateAssetMenu(fileName = "SequenceSO", menuName = "Scriptable Objects/SequenceSO")]
public class SequenceSO : ScriptableObject
{
    [System.Serializable]
    public struct Sequence
    {
        public List<Buttons> ButtonsSequences;
        [Tooltip("Le nombre de fois qu'il faut répéter cette sequence de touches")]
        public int NumberOfRepeats;
    }

    public List<Sequence> ButtonSequenceList;
}
