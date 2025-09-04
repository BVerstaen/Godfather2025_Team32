using System;
using Unity.VisualScripting;
using UnityEngine;
using static ButtonsInputs;
using static SequenceSO;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

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

    public event Action<SequenceSO> OnStartGigaChad;
    public event Action<PlayerSide, Buttons> OnNewInput;
    public event Action<PlayerSide> OnDisableImage;

    private SequenceManager _team1SequenceManager;
    private SequenceManager _team2SequenceManager;

    public SequenceManager Team1SequenceManager
    {
        private get => _team1SequenceManager;
        set
        {
            _team1SequenceManager = value;
            _team1SequenceManager.OnCorrectLeftInput += CorrectLeftInput;
            _team1SequenceManager.OnCorrectRightInput += CorrectRightInput;
            _team1SequenceManager.OnEnterGigaChadMode += GigaChadMode;
            _team1SequenceManager.OnNewInput += SendChangeButton;
            _team1SequenceManager.OnWaitGigaChad += SendDisableImage;
        }
    }
    
    public SequenceManager Team2SequenceManager
    {
        private get => _team2SequenceManager;
        set
        {
            _team2SequenceManager = value;
            _team2SequenceManager.OnCorrectLeftInput += CorrectLeftInput;
            _team2SequenceManager.OnCorrectRightInput += CorrectRightInput;
            _team2SequenceManager.OnEnterGigaChadMode += GigaChadMode;
            _team2SequenceManager.OnNewInput += SendChangeButton;
            _team2SequenceManager.OnWaitGigaChad += SendDisableImage;
        }
    }

    public void TriggerStart() => OnStart?.Invoke();
    public void TriggerAccelerate(float amount) => OnAccelerate?.Invoke(amount);
    public void TriggerMoveLeft(float amount) => OnMoveLeft?.Invoke(amount);
    public void TriggerMoveRight(float amount) => OnMoveRight?.Invoke(amount);

    private void Start()
    {
        Instance = this;
    }

    private void OnDisable()
    {
        if (_team1SequenceManager != null)
        {
            _team1SequenceManager.OnCorrectLeftInput -= CorrectLeftInput;
            _team1SequenceManager.OnCorrectRightInput -= CorrectRightInput;
            _team1SequenceManager.OnEnterGigaChadMode -= GigaChadMode;
            _team1SequenceManager.OnNewInput -= SendChangeButton;
            _team1SequenceManager.OnWaitGigaChad -= SendDisableImage;
        }

        if (_team2SequenceManager != null)
        {
            _team2SequenceManager.OnCorrectLeftInput -= CorrectLeftInput;
            _team2SequenceManager.OnCorrectRightInput -= CorrectRightInput;
            _team2SequenceManager.OnEnterGigaChadMode -= GigaChadMode;
            _team2SequenceManager.OnNewInput -= SendChangeButton;
            _team2SequenceManager.OnWaitGigaChad -= SendDisableImage;
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void GigaChadMode(SequenceSO sequence)
    {
        TriggerAccelerate(10);
        OnStartGigaChad?.Invoke(sequence);
        print("CHADDDDDDDDDDDDDDDDDDDDD");
    }

    private void CorrectLeftInput()
    {
        throw new NotImplementedException();
    }

    private void CorrectRightInput()
    {
        throw new NotImplementedException();
    }

    public void ChangeDifficulty(Team team, SequenceDifficulty difficulty)
    {
        OnChangeDifficulty?.Invoke(team, difficulty);
    }

    private void SendDisableImage(ButtonsInputs.PlayerSide side)
    {
        OnDisableImage?.Invoke(side);
    }

    private void SendChangeButton(ButtonsInputs.PlayerSide side, ButtonsInputs.Buttons buttons)
    {
        OnNewInput?.Invoke(side, buttons);
    }
}
