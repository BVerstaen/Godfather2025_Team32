using System;
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

    private List<SequenceSO> _sequenceList;

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

    public Action OnCorrectLeftInput;
    public Action OnCorrectRightInput;

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
        Debug.LogWarning("Step through sequences after giga chad !");

        CreateSequenceSO();

        //Take the first sequence list
        _leftSideSequence = _sequenceList[0];
        _leftSideCurrentInput = 0;
        _leftSideCurrentRepetition = 0;
        _leftSideCurrentIndex = 0;

        _rightSideSequence = _sequenceList[0];
        _rightSideCurrentInput = 0;
        _rightSideCurrentRepetition = 0;
        _rightSideCurrentIndex = 0;
    }

    private void CreateSequenceSO()
    {
        _sequenceList.Clear();
        for (int i = 0; i < _numberOfSequences; i++)
        {
            _sequenceList.Add(_possibleSequences[Random.Range(0, _possibleSequences.Count)]);
        }
        return _sequenceList;
    }

    private SequenceSO ChoseRandomSequence() => _sequenceList[Random.Range(0, _sequenceList.Count)];

    private void ButtonPressed(ButtonsInputs.PlayerSide side, ButtonsInputs.Buttons buttons)
    {
        switch (side)
        {
            case ButtonsInputs.PlayerSide.Left:
                {
                    if (_hasLeftFinishedSequence)
                        return;

                    Sequence currentSequence = _leftSideSequence.ButtonSequenceList[_leftSideCurrentIndex];
                    if (buttons == currentSequence.ButtonsSequences[_leftSideCurrentInput])
                    {
                        _leftSideCurrentInput++;
                        if (_leftSideCurrentInput >= currentSequence.ButtonsSequences.Count) //Finished index list
                        {
                            print("Left finished inputs");
                            _leftSideCurrentInput = 0;
                            _leftSideCurrentRepetition++;
                            if(_leftSideCurrentRepetition >= currentSequence.NumberOfRepeats) //Reach number of repeats
                            {
                                print("Left finished repetition");
                                _leftSideCurrentRepetition = 0;
                                _leftSideCurrentIndex++;
                                if(_leftSideCurrentIndex >= _leftSideSequence.ButtonSequenceList.Count) //Finished sequence
                                {
                                    _leftSideCurrentInput = 0;
                                    _hasLeftFinishedSequence = true;
                                    print("Left sequence finished");
                                }
                            }
                        }
                    }
                    break;
                }

            case ButtonsInputs.PlayerSide.Right:
                {
                    if (_hasRightFinishedSequence)
                        return;

                    Sequence currentSequence = _rightSideSequence.ButtonSequenceList[_rightSideCurrentIndex];
                    if (buttons == currentSequence.ButtonsSequences[_rightSideCurrentInput])
                    {
                        _rightSideCurrentInput++;
                        if (_rightSideCurrentInput >= currentSequence.ButtonsSequences.Count) //Finished index list
                        {
                            print("Right finished inputs");
                            _rightSideCurrentInput = 0;
                            _rightSideCurrentRepetition++;
                            if (_rightSideCurrentRepetition >= currentSequence.NumberOfRepeats) //Reach number of repeats
                            {
                                print("Right finished repetition");
                                _rightSideCurrentRepetition = 0;
                                _rightSideCurrentIndex++;
                                if (_rightSideCurrentIndex >= _rightSideSequence.ButtonSequenceList.Count) //Finished sequence
                                {
                                    _rightSideCurrentIndex = 0;
                                    _hasRightFinishedSequence = true;
                                    print("Right sequence finished");
                                }
                            }
                        }
                    }
                    break;
                }
        }
    }

    private void OnCircularMovement(CircularMovementDetector.StickType type, CircularMovementDetector.RotationDirection direction)
    {
        throw new NotImplementedException();
    }
}
