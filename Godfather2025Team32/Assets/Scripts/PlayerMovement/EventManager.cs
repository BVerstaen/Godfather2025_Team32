using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    
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

    public void TriggerStart() => OnStart?.Invoke();
    public void TriggerAccelerate(float amount) => OnAccelerate?.Invoke(amount);
    public void TriggerMoveLeft(float amount) => OnMoveLeft?.Invoke(amount);
    public void TriggerMoveRight(float amount) => OnMoveRight?.Invoke(amount);

    private void Start()
    {
        sequenceManager.OnCorrectLeftInput += CorrectLeftInput;
        sequenceManager.OnEnterGigaChadMode += GigaChadMode;   
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
    
}
