using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SequenceSO;
using Random = UnityEngine.Random;

public class SequenceManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ButtonsInputs _buttonInputs;
    [SerializeField] private CircularMovementDetector _circularMovementDetector;

    [Header("Sequence")]
    [SerializeField] private List<SequenceSO> _possibleSequences;
    [SerializeField] private int _numberOfSequences;

    [Header("Giga chad")]
    [SerializeField] private float _gigaChadDuration;
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

    public Action OnCorrectLeftInput;
    public Action OnCorrectRightInput;

    public Action OnEnterGigaChadMode;
    public Action OnExitGigaChadMode;

    private void OnEnable()
    {
        _buttonInputs.OnButtonPressed += ButtonPressed;
        _circularMovementDetector.OnDetectCircularMovement += OnCircularMovement;
    }

    private void OnDisable()
    {
        _buttonInputs.OnButtonPressed -= ButtonPressed;
        _circularMovementDetector.OnDetectCircularMovement -= OnCircularMovement;
    }

    private void Awake()
    {
        GiveNewRandomSequence();
    }

    private void GiveNewRandomSequence()
    {
        SequenceSO newSequence = _possibleSequences[Random.Range(0, _possibleSequences.Count)];
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
    }

    private void ButtonPressed(ButtonsInputs.PlayerSide side, ButtonsInputs.Buttons buttons)
    {
        bool isLeft = side == ButtonsInputs.PlayerSide.Left;

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
                        CheckToLaunchGigaChad();
                    }
                }
            }
        }
    }


    private void CheckToLaunchGigaChad()
    {
        if (!_hasLeftFinishedSequence || !_hasRightFinishedSequence)
            return;

        Debug.Log("Enter Giga Chad !");
        _gigaChadCoroutine = StartCoroutine(GigaChadRoutine());
    }

    private void EndGigaChad()
    {
        if(_gigaChadCoroutine != null)
        {
            StopCoroutine(_gigaChadCoroutine);
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
            OnCorrectLeftInput?.Invoke();

            if(_leftInactiveCoolDown != null)
            {
                StopCoroutine(_leftInactiveCoolDown);
                _leftInactiveCoolDown = null;
            }
            _leftInactiveCoolDown = StartCoroutine(InactiveCoolDown());
        }
        else if (!isLeft && direction == _rightSideSequence.RightGigaChadRotation)
        {
            OnCorrectRightInput?.Invoke();

            if (_rightInactiveCoolDown != null)
            {
                StopCoroutine(_rightInactiveCoolDown);
                _rightInactiveCoolDown = null;
            }
            _rightInactiveCoolDown = StartCoroutine(InactiveCoolDown());
        }
    }

    private IEnumerator InactiveCoolDown()
    {
        yield return new WaitForSeconds(_inactiveTimeDelay);
        Debug.Log("Stop Giga Chad (inactive cooldown)!");
        EndGigaChad();
    }
}
