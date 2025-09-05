using UnityEngine;

public class BetsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _mainCanvas;
    [SerializeField] private GameObject _controllerManager;

    private void Start()
    {
        if (!GameManager.Instance.IsFirstRound())
        {
            _controllerManager.SetActive(true);
            _mainCanvas.SetActive(false);
            return;
        }

        _controllerManager.SetActive(false);
    }

    public void CloseBets()
    {
        _controllerManager.SetActive(true);
        _mainCanvas.SetActive(false);
    }
}
