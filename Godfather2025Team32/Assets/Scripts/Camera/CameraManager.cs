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

    [SerializeField] private float _centralCameraDistance = 1000;
    [SerializeField] private float _centralCameraHeight = 10;

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

    private void OnEnable()
    {
        EventManager.Instance.OnStart += PlaceCameras;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnStart -= PlaceCameras;
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayersPos();
        /*if (!_isCameraSplit) MoveCentralCamera();
        else MoveSplitCameras();*/
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

    public void PlaceCameras()
    {
        PlaceCentralCamera();
        PlaceSplitsCameras();
    }

    private void PlaceCentralCamera()
    {
        _centralCamera.transform.SetParent(_leftPlayer.transform);

        _centralCamera.transform.position = _leftPlayer.transform.position + _stickDistance;
    }

    private void PlaceSplitsCameras()
    {
        _rightCamera.transform.SetParent(_rightPlayer.transform);
        _leftCamera.transform.SetParent(_leftPlayer.transform);

        _rightCamera.transform.position = _rightPlayer.transform.position + _stickDistance;
        _leftCamera.transform.position = _leftPlayer.transform.position + _stickDistance;
    }

    private void MoveCentralCamera()
    {
        if (_leftPlayer == null || _rightPlayer == null)
            return;

        /*Vector3 cameraPos = _offSetPos;
        Vector3 playersOffSet = _rightPlayer.transform.position + _leftPlayer.transform.position;
        cameraPos += playersOffSet * 0.5f;
        _centralCamera.transform.position = cameraPos + _stickDistance;*/


        Vector3 playerVector = _rightPlayer.transform.position - _leftPlayer.transform.position;
        Vector3 midPoint = playerVector * 0.5f + _leftPlayer.transform.position;
        //Vector3 perpendicular = playerVector.normalized * Vector3.right;
        Vector3 perpendicular = new Vector3(0, -playerVector.y, 0).normalized;
        Vector3 cameraPos = midPoint + perpendicular * _centralCameraDistance;
        cameraPos.y += _centralCameraHeight;
        _centralCamera.transform.eulerAngles = _stickEulerAngles;
        _centralCamera.transform.position = cameraPos;
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
