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

    void Awake()
    {
        ApplyNullosSprite(_playerController.currentTeam);
    }

    private void OnEnable()
    {
        EventManager.Instance.OnEndGigaChad += ApplyNullosSprite;
        EventManager.Instance.OnStartGigaChad += ApplyRandomChadSprite;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnEndGigaChad -= ApplyNullosSprite;
        EventManager.Instance.OnStartGigaChad -= ApplyRandomChadSprite;
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

    }
}
