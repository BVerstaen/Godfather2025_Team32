using System;
using TMPro;
using UnityEngine;

public class PrepareUI : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject _mainCanvas;
    [SerializeField] private TMP_Text _roundText;
    [SerializeField] private GameObject _gameObjectText1;
    [SerializeField] private GameObject _gameObjectText2;

    private void Start()
    {
        _roundText.text = "Round : " + (GameManager.Instance.CurrentRound + 1);
        EventManager.Instance.OnLeftPlayerPrepared += DisableLeftText;
        EventManager.Instance.OnRightPlayerPrepared += DisableRightText;
        EventManager.Instance.OnStart += DisableCanvas;
    }

    private void DisableCanvas()
    {
        _mainCanvas.SetActive(false);
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
