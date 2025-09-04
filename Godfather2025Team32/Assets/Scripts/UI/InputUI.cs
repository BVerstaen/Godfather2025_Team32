using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ButtonsInputs;

public class InputUI : MonoBehaviour
{
    [System.Serializable]
    private struct SpriteToImage
    {
        public Buttons Button;
        public Sprite Sprite;
    }

    [Header("References")]
    [SerializeField] private Team _currentTeam;

    [SerializeField] private Image _spriteImage;

    [Header("Sprites List")]
    [SerializeField] private PlayerSide _currentSide;
    [Space(5)]
    [SerializeField] private List<SpriteToImage> _spriteList;
    [SerializeField] private Sprite _gigaChadClockwiseImage;
    [SerializeField] private Sprite _gigaChadCounterClockwiseImage;


    [Header("Animation")]
    [SerializeField] private AnimationCurve _buttonSizeAnimationCurve;
    [SerializeField] private float _buttonAnimationSpeed;
    [SerializeField] private float _buttonAnimationMaxSize;

    private RectTransform _rect => GetComponent<RectTransform>();
    private Coroutine _animationCoroutine;
    
    private void OnEnable()
    {
        EventManager.Instance.OnStartGigaChad += StartGigaChadUI;
        EventManager.Instance.OnNewInput += ChangeButton;
        EventManager.Instance.OnDisableImage += DisableImage;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnStartGigaChad -= StartGigaChadUI;
        EventManager.Instance.OnNewInput -= ChangeButton;
        EventManager.Instance.OnDisableImage -= DisableImage;
    }

    private void Update()
    {
        float newScale = _buttonSizeAnimationCurve.Evaluate(Mathf.PingPong(Time.time * _buttonAnimationSpeed, 1));
        newScale = Mathf.Lerp(1,_buttonAnimationMaxSize, newScale);
        _rect.localScale = new Vector2(newScale, newScale);
    }

    private void ChangeButton(PlayerSide side, Buttons newButton)
    {
        if (side != _currentSide)
            return;

        _spriteImage.enabled = true;
        foreach (SpriteToImage spriteToButton in _spriteList)
        {
            if(newButton == spriteToButton.Button)
            {
                _spriteImage.sprite = spriteToButton.Sprite;
                return;
            }
        }
    }

    private void DisableImage(PlayerSide side)
    {
        if (side != _currentSide)
            return;

        _spriteImage.enabled = false;
    }

    private void StartGigaChadUI(SequenceSO sequence)
    {
        _spriteImage.enabled = true;
        switch (_currentSide)
        {
            case PlayerSide.Left:
                _spriteImage.sprite = sequence.LeftGigaChadRotation == CircularMovementDetector.RotationDirection.Clockwise ? _gigaChadClockwiseImage : _gigaChadCounterClockwiseImage;
                break;

            case PlayerSide.Right:
                _spriteImage.sprite = sequence.RightGigaChadRotation == CircularMovementDetector.RotationDirection.Clockwise ? _gigaChadClockwiseImage : _gigaChadCounterClockwiseImage;
                break;
        }
    }
}
