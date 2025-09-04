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

    [Header("Animation")]
    [SerializeField] private AnimationCurve _buttonSizeAnimationCurve;
    [SerializeField] private float _buttonAnimationSpeed;
    [SerializeField] private float _buttonAnimationMaxSize;

    [Header("TurnSpriteList")]
    [SerializeField] private List<Sprite> _joystickTurnSpriteList;
    [SerializeField] private float _joystickAnimationTurnSpeed;

    private float _joystickDirection;
    private int _direction = 1;
    private bool _bindToEvent;
    private RectTransform _rect => GetComponent<RectTransform>();
    private Coroutine _animationCoroutine;
    private Coroutine _joystickAnimationRoutine;

    private void OnEnable()
    {
        EventManager.Instance.OnStartGigaChad += StartGigaChadUI;
        EventManager.Instance.OnNewInput += ChangeButton;
        EventManager.Instance.OnDisableImage += DisableImage;
        _bindToEvent = true;
    }

    private void OnDisable()
    {
        if (_bindToEvent)
            return;

        EventManager.Instance.OnStartGigaChad -= StartGigaChadUI;
        EventManager.Instance.OnNewInput -= ChangeButton;
        EventManager.Instance.OnDisableImage -= DisableImage;
    }

    private void Update()
    {
        float newScale = _buttonSizeAnimationCurve.Evaluate(Mathf.PingPong(Time.time * _buttonAnimationSpeed, 1));
        newScale = Mathf.Lerp(1, _buttonAnimationMaxSize, newScale);
        _rect.localScale = new Vector2(_direction * newScale, newScale);
    }

    private void ChangeButton(Team team, PlayerSide side, Buttons newButton)
    {

        if (team != _currentTeam)
            return;

        if (side != _currentSide)
            return;

        if(_joystickAnimationRoutine != null)
        {
            StopCoroutine(_joystickAnimationRoutine);
            _joystickAnimationRoutine = null;
        }


        _spriteImage.enabled = true;
        _direction = 1;
        foreach (SpriteToImage spriteToButton in _spriteList)
        {
            if (newButton == spriteToButton.Button)
            {
                _spriteImage.sprite = spriteToButton.Sprite;
                return;
            }
        }
    }

    private void DisableImage(Team team, PlayerSide side)
    {
        if (team != _currentTeam)
            return;

        if (side != _currentSide)
            return;

        _spriteImage.enabled = false;
    }

    private void StartGigaChadUI(Team team, SequenceSO sequence)
    {
        if (team != _currentTeam)
            return;

        _spriteImage.enabled = true;
        int direction;
        switch (_currentSide)
        {
            case PlayerSide.Left:
                direction = sequence.LeftGigaChadRotation == CircularMovementDetector.RotationDirection.Clockwise ? 1 : -1;
                _direction = direction;
                _joystickAnimationRoutine = StartCoroutine(JoystickImageAnimation());
                break;

            case PlayerSide.Right:
                direction = sequence.RightGigaChadRotation == CircularMovementDetector.RotationDirection.Clockwise ? 1 : -1;
                _direction = direction;
                _joystickAnimationRoutine = StartCoroutine(JoystickImageAnimation());
                break;
        }
    }

    private IEnumerator JoystickImageAnimation()
    {
        int spriteIndex = 0 ;
        while (true)
        {
            _spriteImage.sprite = _joystickTurnSpriteList[spriteIndex];
            
            spriteIndex++;
            if (spriteIndex == _joystickTurnSpriteList.Count)
                spriteIndex = 0;
            yield return new WaitForSeconds(_joystickAnimationTurnSpeed);
        }
    }
}
