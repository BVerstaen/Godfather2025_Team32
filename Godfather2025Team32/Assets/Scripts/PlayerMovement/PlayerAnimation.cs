using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer _sr;
    [Space]
    [SerializeField] private PlayerController _playerController;

    [Header("Sprites")]
    [SerializeField] private Sprite _nullosSprite;
    [SerializeField] private Sprite _LeftFinishedSprite;
    [SerializeField] private Sprite _RightFinishedSprite;

    [SerializeField] private Sprite _chadSprite;

    void Awake()
    {
        ApplyNullosSprite(_playerController.currentTeam);
    }

    private void OnEnable()
    {
        EventManager.Instance.OnEndGigaChad += ApplyNullosSprite;
        EventManager.Instance.OnStartGigaChad += ApplyRandomChadSprite;

        EventManager.Instance.OnLeftPlayerFinished += ApplyUpperHalfChadSprite;
        EventManager.Instance.OnRightPlayerFinished += ApplyLowerHalfChadSprite;
    }


    private void ApplyUpperHalfChadSprite(Team team)
    {
        if (_playerController.currentTeam != team)
            return;

        _sr.sprite = _LeftFinishedSprite;
    }
    private void ApplyLowerHalfChadSprite(Team team)
    {
        if (_playerController.currentTeam != team)
            return;

        _sr.sprite = _RightFinishedSprite;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnEndGigaChad -= ApplyNullosSprite;
        EventManager.Instance.OnStartGigaChad -= ApplyRandomChadSprite;

        EventManager.Instance.OnLeftPlayerFinished -= ApplyUpperHalfChadSprite;
        EventManager.Instance.OnRightPlayerFinished -= ApplyLowerHalfChadSprite;
    }

    private void ApplyNullosSprite(Team team)
    {
        if (_playerController.currentTeam != team)
            return;

        _sr.sprite = _nullosSprite;
    }

    private void ApplyRandomChadSprite(Team team, SequenceSO sO)
    {
        if (_playerController.currentTeam != team)
            return;

        Sprite newSprite;
        newSprite = MarketManager.Instance.GetRandomUnlockedSprite(team);

        if (newSprite)
            _sr.sprite = newSprite;
        else
            _sr.sprite = _chadSprite;
    }
}
