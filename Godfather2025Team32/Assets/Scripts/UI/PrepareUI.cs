using System;
using TMPro;
using UnityEngine;

public class PrepareUI : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private TMP_Text _roundText;
    [SerializeField] private GameObject _gameObjectText1;
    [SerializeField] private GameObject _gameObjectText2;

    private void Start()
    {
        _roundText.text = "Round : " + GameManager.Instance.CurrentRound;
        EventManager.Instance.OnLeftPlayerPrepared += DisableLeftText;
        EventManager.Instance.OnRightPlayerPrepared += DisableRightText;
    }

    private void DisableLeftText()
    {
        _gameObjectText1.SetActive(false);
    }

    private void DisableRightText()
    {
        _gameObjectText2.SetActive(false);
    }
}
