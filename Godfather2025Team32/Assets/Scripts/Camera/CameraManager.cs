using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    //---------- VARIABLES ----------\\

    [SerializeField] private Camera _centralCamera;
    [SerializeField] private Camera _leftCamera;
    [SerializeField] private Camera _rightCamera;
    [SerializeField] private float _maxPlayerDistance;

    [Space]
    [SerializeField] private Vector3 _stickDistance;
    [SerializeField] private Vector3 _stickEulerAngles;
    //[SerializeField] private Vector3 _offSetPos;
    
    private Vector3 _offSetPos;
    private GameObject _leftPlayer;
    private GameObject _rightPlayer;

    public GameObject LeftPlayer { set => _leftPlayer = value; }
    public GameObject RightPlayer { set => _rightPlayer = value; }
    

    private bool _isCameraSplit = false;

    //---------- FUNCTIONS ----------\\

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _leftCamera.gameObject.SetActive(false);
        _rightCamera.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayersPos();
        if (!_isCameraSplit) MoveCentralCamera();
        else MoveSplitCameras();
        //_leftPlayer.gameObject.transform.position += new Vector3(-0.5f, 0, 0);
    }

    private void CheckPlayersPos()
    {
        if (_leftPlayer == null || _rightPlayer == null)
            return;

        Vector3 playersVector = _rightPlayer.transform.position - _leftPlayer.transform.position;
        float playersDistance = playersVector.magnitude;
        bool shouldSplit = playersDistance >= _maxPlayerDistance;
        if (shouldSplit != _isCameraSplit)
        {
            if (_isCameraSplit)
            {
                MergeCamera();
            }
            else
            {
                SplitCamera();
            }
            _isCameraSplit = shouldSplit;
        }
    }

    private void MergeCamera()
    {
        if (_leftPlayer == null || _rightPlayer == null)
            return;

        _leftCamera.gameObject.SetActive(false);
        _rightCamera.gameObject.SetActive(false);
        _centralCamera.gameObject.SetActive(true);
    }

    private void SplitCamera()
    {
        if (_leftPlayer == null || _rightPlayer == null)
            return;

        _leftCamera.gameObject.SetActive(true);
        _rightCamera.gameObject.SetActive(true);
        _centralCamera.gameObject.SetActive(false);
    }

    private void MoveCentralCamera()
    {
        if (_leftPlayer == null || _rightPlayer == null)
            return;

        Vector3 cameraPos = _offSetPos;
        Vector3 playersOffSet = _rightPlayer.transform.position + _leftPlayer.transform.position;
        cameraPos += playersOffSet*0.5f;
        _centralCamera.transform.position = cameraPos + _stickDistance;
        _centralCamera.transform.eulerAngles = _stickEulerAngles;
    }

    private void MoveSplitCameras()
    {
        if (_leftPlayer == null || _rightPlayer == null)
            return;

        Vector3 leftPos = _leftPlayer.transform.position + _offSetPos;
        Vector3 rightPos = _rightPlayer.transform.position + _offSetPos;
        _leftCamera.transform.position = leftPos + _stickDistance;
        _leftCamera.transform.LookAt(leftPos);
        _rightCamera.transform.position = rightPos + _stickDistance;
        _rightCamera.transform.LookAt(rightPos);
    }
}
