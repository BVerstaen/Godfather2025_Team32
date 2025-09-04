using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    //---------- VARIABLES ----------\\

    [SerializeField] private Canvas _canvas;
    [SerializeField] private RawImage _chadImage;
    [SerializeField] private GameObject _markerPlayButton;
    [SerializeField] private Button _playButton;
    private Vector3 _playButtonFinalPos;
    [SerializeField] private float _speed = 1;
    private Vector3 _chadDirection;
    private bool _isChadMoving;
    private float _playButtonPosTreshold = 5;

    //---------- FUNCTIONS ----------\\

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playButtonFinalPos = _playButton.transform.position;
        _playButton.transform.parent = _markerPlayButton.transform;
        _playButton.transform.localPosition = Vector3.zero;
        _chadDirection = _playButtonFinalPos - _playButton.transform.position;
        _isChadMoving = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isChadMoving)
        {
            _chadImage.transform.position += _chadDirection * _speed * Time.deltaTime;
            if ((_playButtonFinalPos - _playButton.transform.position).magnitude < _playButtonPosTreshold)
            {
                _playButton.transform.position = _playButtonFinalPos;
                _playButton.transform.parent = _canvas.transform;
//                _isChadMoving = false;
            }
        }
    }
}