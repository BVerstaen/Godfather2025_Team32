using System;
using UnityEngine;
using static SequenceSO;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    
    [SerializeField]
    private SequenceManager sequenceManager;
    
    [HideInInspector]
    public event Action OnStart;

    [HideInInspector]
    public event Action<float> OnAccelerate;
    
    [HideInInspector]
    public event Action<float> OnMoveLeft;
    
    [HideInInspector]
    public event Action<float> OnMoveRight;

    [HideInInspector]
    public event Action<Team, SequenceDifficulty> OnChangeDifficulty;

    public void TriggerStart() => OnStart?.Invoke();
    public void TriggerAccelerate(float amount) => OnAccelerate?.Invoke(amount);
    public void TriggerMoveLeft(float amount) => OnMoveLeft?.Invoke(amount);
    public void TriggerMoveRight(float amount) => OnMoveRight?.Invoke(amount);

    private void Start()
    {
        Instance = this;

        sequenceManager.OnCorrectLeftInput += CorrectLeftInput;
        sequenceManager.OnEnterGigaChadMode += GigaChadMode;   
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void GigaChadMode()
    {
        TriggerAccelerate(10);
        print("CHADDDDDDDDDDDDDDDDDDDDD");
    }

    private void CorrectLeftInput()
    {
        print("CorrectLeftInput");
    }

    public void ChangeDifficulty(Team team, SequenceDifficulty difficulty)
    {
        OnChangeDifficulty?.Invoke(team, difficulty);
    }
}
