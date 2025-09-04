using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ButtonsInputs;
using static SequenceSO;
using Random = UnityEngine.Random;

public class SequenceManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private MonoTeamManager _teamManager;
    [SerializeField] private ButtonsInputs _buttonInputs;
    [SerializeField] private CircularMovementDetector _circularMovementDetector;

    [Header("Sequence")]
    [SerializeField] private List<SequenceSO> _possibleSequences;
    [SerializeField] private int _numberOfSequences;
    [Space(5)]
    [SerializeField] private SequenceDifficulty _currentDifficulty;

    [Header("Giga chad")]
    [SerializeField] private float _gigaChadDuration;
    [SerializeField] private float _firstinactiveCoolDown;
    [SerializeField] private float _inactiveTimeDelay;

    private SequenceSO _leftSideSequence;
    private SequenceSO _rightSideSequence;

    private int _leftSideCurrentInput; //Quel sequence est joué
    private int _leftSideCurrentRepetition; //Combien de fois l'input a été répétée
    private int _leftSideCurrentIndex; //Quel input doit être appuyé
    private bool _hasLeftFinishedSequence;

    private int _rightSideCurrentInput;//Quel sequence est joué
    private int _rightSideCurrentRepetition; //Combien de fois l'input a été répétée
    private int _rightSideCurrentIndex;//Quel input doit être appuyé
    private bool _hasRightFinishedSequence;

    private Coroutine _gigaChadCoroutine;
    private Coroutine _leftInactiveCoolDown;
    private Coroutine _rightInactiveCoolDown;

    public SequenceSO GigaChadSequence { get => _leftSideSequence; }

    public Action OnCorrectLeftInput;
    public Action OnCorrectRightInput;

    public Action<SequenceSO> OnEnterGigaChadMode;
    public Action OnExitGigaChadMode;

    public Action<PlayerSide, Buttons> OnNewInput;
    public Action<PlayerSide> OnWaitGigaChad;

    private void OnEnable()
    {
        //ControllerManager.Instance.AddSequenceManager(this);
        _buttonInputs.OnButtonPressed += ButtonPressed;
        _circularMovementDetector.OnDetectCircularMovement += OnCircularMovement;

        //EventManager.Instance.OnChangeDifficulty += ChangeDifficulty;
    }

    private void OnDisable()
    {
        _buttonInputs.OnButtonPressed -= ButtonPressed;
        _circularMovementDetector.OnDetectCircularMovement -= OnCircularMovement;

        //EventManager.Instance.OnChangeDifficulty -= ChangeDifficulty;
    }

    private void ChangeDifficulty(Team team, SequenceDifficulty difficulty)
    {
        if (_teamManager.CurrentTeam != team)
            return;

        _currentDifficulty = difficulty;
    }

    private void Awake()
    {
        GiveNewRandomSequence();
    }

    private void GiveNewRandomSequence()
    {
        SequenceSO newSequence = _possibleSequences[Random.Range(0, _possibleSequences.Count)];
        int guardWhile = 0;
        //Select from difficulty
        while(newSequence.Difficulty != _currentDifficulty)
        {
            newSequence = _possibleSequences[Random.Range(0, _possibleSequences.Count)];

            guardWhile++;
            if(guardWhile >= 100)
            {
                Debug.Log("Couldn't find from difficulty");
                newSequence = _possibleSequences[Random.Range(0, _possibleSequences.Count)];
                break;
            }

        }


        _leftSideSequence = newSequence;
        _rightSideSequence = newSequence;

        //Reset left sequence
        _leftSideCurrentInput = 0;
        _leftSideCurrentRepetition = 0;
        _leftSideCurrentIndex = 0;
        _hasLeftFinishedSequence = false;

        //Reset right sequence
        _rightSideCurrentInput = 0;
        _rightSideCurrentRepetition = 0;
        _rightSideCurrentIndex = 0;
        _hasRightFinishedSequence = false;

        OnNewInput?.Invoke(PlayerSide.Left, _leftSideSequence.ButtonSequenceList[0].ButtonsSequences[0]);
        OnNewInput?.Invoke(PlayerSide.Right, _rightSideSequence.ButtonSequenceList[0].ButtonsSequences[0]);
    }

    private void ButtonPressed(PlayerSide side, Buttons buttons)
    {
        bool isLeft = side == PlayerSide.Left;

        if ((isLeft && _hasLeftFinishedSequence) || (!isLeft && _hasRightFinishedSequence))
            return;

        // Récupération des variables en fonction du côté
        var sequenceList = isLeft ? _leftSideSequence.ButtonSequenceList : _rightSideSequence.ButtonSequenceList;
        ref int currentIndex = ref isLeft ? ref _leftSideCurrentIndex : ref _rightSideCurrentIndex;
        ref int currentInput = ref isLeft ? ref _leftSideCurrentInput : ref _rightSideCurrentInput;
        ref int currentRepetition = ref isLeft ? ref _leftSideCurrentRepetition : ref _rightSideCurrentRepetition;
        ref bool hasFinishedSequence = ref isLeft ? ref _hasLeftFinishedSequence : ref _hasRightFinishedSequence;

        Sequence currentSequence = sequenceList[currentIndex];

        if (buttons == currentSequence.ButtonsSequences[currentInput])
        {
            currentInput++;
            if (currentInput >= currentSequence.ButtonsSequences.Count) // Finished input list
            {
                //Debug.Log($"{side} finished inputs");
                currentInput = 0;
                currentRepetition++;

                if (currentRepetition >= currentSequence.NumberOfRepeats) // Finished repetitions
                {
                    Debug.Log($"{side} finished repetition");
                    currentRepetition = 0;
                    currentIndex++;

                    if (currentIndex >= sequenceList.Count) // Finished full sequence
                    {
                        Debug.Log($"{side} sequence finished");
                        currentIndex = 0;
                        hasFinishedSequence = true;
                        OnWaitGigaChad?.Invoke(side);
                        CheckToLaunchGigaChad();
                        return;
                    }
                }
            }

            //Update UI
            Sequence newSequence = sequenceList[currentIndex];
            OnNewInput?.Invoke(side, newSequence.ButtonsSequences[currentInput]);
        }
    }


    private void CheckToLaunchGigaChad()
    {
        if (!_hasLeftFinishedSequence || !_hasRightFinishedSequence)
            return;

        Debug.Log("Enter Giga Chad !");
        OnEnterGigaChadMode?.Invoke(GigaChadSequence);
        _gigaChadCoroutine = StartCoroutine(GigaChadRoutine());
        _leftInactiveCoolDown = StartCoroutine(InactiveCoolDown(_firstinactiveCoolDown));
        _rightInactiveCoolDown = StartCoroutine(InactiveCoolDown(_firstinactiveCoolDown));

    }

    private void EndGigaChad()
    {
        if(_gigaChadCoroutine != null)
        {
            StopCoroutine(_gigaChadCoroutine);
        }

        if (_leftInactiveCoolDown != null)
        {
            StopCoroutine(_leftInactiveCoolDown);
            _leftInactiveCoolDown = null;
        }

        if (_rightInactiveCoolDown != null)
        {
            StopCoroutine(_rightInactiveCoolDown);
            _rightInactiveCoolDown = null;
        }

        OnExitGigaChadMode?.Invoke();
        GiveNewRandomSequence();
    }

    private IEnumerator GigaChadRoutine()
    {
        yield return new WaitForSeconds(_gigaChadDuration);
        Debug.Log("Stop Giga Chad (duration ended)!");
        EndGigaChad();
    }

    private void OnCircularMovement(CircularMovementDetector.StickType type, CircularMovementDetector.RotationDirection direction)
    {
        if (!_hasLeftFinishedSequence || !_hasRightFinishedSequence)
            return;

        bool isLeft = type == CircularMovementDetector.StickType.LeftStick;

        // Vérifie la bonne direction et invoque l'événement correspondant
        if (isLeft && direction == _leftSideSequence.LeftGigaChadRotation)
        {
            if (_leftInactiveCoolDown != null)
            {
                StopCoroutine(_leftInactiveCoolDown);
                _leftInactiveCoolDown = null;
            }
            _leftInactiveCoolDown = StartCoroutine(InactiveCoolDown(_inactiveTimeDelay, ButtonsInputs.PlayerSide.Left));
        }
        else if (!isLeft && direction == _rightSideSequence.RightGigaChadRotation)
        {
            if (_rightInactiveCoolDown != null)
            {
                StopCoroutine(_rightInactiveCoolDown);
                _rightInactiveCoolDown = null;
            }
            _rightInactiveCoolDown = StartCoroutine(InactiveCoolDown(_inactiveTimeDelay, ButtonsInputs.PlayerSide.Right));
        }
    }

    private IEnumerator InactiveCoolDown(float duration, ButtonsInputs.PlayerSide side = ButtonsInputs.PlayerSide.Left)
    {
        yield return new WaitForSeconds(duration);
        Debug.Log($"Stop Giga Chad (inactive cooldown) for {side}!");
        EndGigaChad();
    }
}
