using System;
using System.Collections;
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
    public event Action<Team, float> OnAccelerate;

    [HideInInspector]
    public event Action<Team> OnMoveLeft;

    [HideInInspector]
    public event Action<Team> OnMoveRight;

    [HideInInspector]
    public event Action<Team, SequenceDifficulty> OnChangeDifficulty;

    public event Action<Team, SequenceSO> OnStartGigaChad;
    public event Action<Team> OnEndGigaChad;

    public event Action<Team, PlayerSide, Buttons> OnNewInput;
    public event Action<Team, PlayerSide> OnDisableImage;

    public event Action OnLeftPlayerPrepared;
    public event Action OnRightPlayerPrepared;

    public event Action<Team> OnLeftPlayerFinished;
    public event Action<Team> OnRightPlayerFinished;

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

            OnLeftPlayerPrepared?.Invoke();
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

            //Start when second player is connected
            //Assume 2nd player is last to be connected
            OnRightPlayerPrepared?.Invoke();
            GameManager.Instance.ResetGame();
            TriggerStart();
        }
    }

    public void TriggerStart() => StartCoroutine(WaitAndTrigger());
    private IEnumerator WaitAndTrigger()
    {
        yield return new WaitForSeconds(.1f);
        OnStart?.Invoke();
    }
    public void TriggerAccelerate(Team team, float amount) => OnAccelerate?.Invoke(team, amount);
    public void TriggerMoveLeft(Team team) => OnMoveLeft?.Invoke(team);
    public void TriggerMoveRight(Team team) => OnMoveRight?.Invoke(team);

    private void Awake()
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
            _team1SequenceManager.OnExitGigaChadMode += ExitGigaChadMode;
        }

        if (_team2SequenceManager != null)
        {
            _team2SequenceManager.OnCorrectLeftInput -= CorrectLeftInput;
            _team2SequenceManager.OnCorrectRightInput -= CorrectRightInput;
            _team2SequenceManager.OnEnterGigaChadMode -= GigaChadMode;
            _team2SequenceManager.OnNewInput -= SendChangeButton;
            _team2SequenceManager.OnWaitGigaChad -= SendDisableImage;
            _team1SequenceManager.OnExitGigaChadMode -= ExitGigaChadMode;
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void GigaChadMode(Team team, SequenceSO sequence)
    {
        TriggerAccelerate(team, 10);
        OnStartGigaChad?.Invoke(team, sequence);
        
        print("CHADDDDDDDDDDDDDDDDDDDDD");
    }

    private void ExitGigaChadMode(Team team) => OnEndGigaChad?.Invoke(team);

    private void CorrectLeftInput(Team team)
    {
        TriggerMoveLeft(team);
    }

    private void CorrectRightInput(Team team)
    {
        TriggerMoveRight(team);
    }

    public void ChangeDifficulty(Team team, SequenceDifficulty difficulty)
    {
        OnChangeDifficulty?.Invoke(team, difficulty);
    }

    private void SendDisableImage(Team team, ButtonsInputs.PlayerSide side)
    {
        OnDisableImage?.Invoke(team, side);
    }

    private void SendChangeButton(Team team,  ButtonsInputs.PlayerSide side, ButtonsInputs.Buttons buttons)
    {
        OnNewInput?.Invoke(team, side, buttons);
    }

    
}
