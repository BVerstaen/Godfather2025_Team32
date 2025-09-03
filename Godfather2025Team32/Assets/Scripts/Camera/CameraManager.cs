using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    //---------- VARIABLES ----------\\

    [SerializeField] private Camera _centralCamera;
    [SerializeField] private Camera _leftCamera;
    [SerializeField] private Camera _rightCamera;
    [SerializeField] private GameObject _leftPlayer;
    [SerializeField] private GameObject _rightPlayer;
    [SerializeField] private float _maxPlayerDistance;
    //[SerializeField] private Vector3 _offSetPos;
    private Vector3 _offSetPos;

    private bool _isCameraSplit = false;

    //---------- FUNCTIONS ----------\\

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 playersPos = (_rightPlayer.transform.position + _leftPlayer.transform.position) * 0.5f;
        _offSetPos = _centralCamera.transform.position - playersPos;
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
        _leftCamera.gameObject.SetActive(false);
        _rightCamera.gameObject.SetActive(false);
        _centralCamera.gameObject.SetActive(true);
    }

    private void SplitCamera()
    {
        _leftCamera.gameObject.SetActive(true);
        _rightCamera.gameObject.SetActive(true);
        _centralCamera.gameObject.SetActive(false);
    }

    private void MoveCentralCamera()
    {
        Vector3 cameraPos = _offSetPos;
        Vector3 playersOffSet = _rightPlayer.transform.position + _leftPlayer.transform.position;
        cameraPos += playersOffSet*0.5f;
        _centralCamera.transform.position = cameraPos;
    }

    private void MoveSplitCameras()
    {
        Vector3 leftPos = _leftPlayer.transform.position + _offSetPos;
        Vector3 rightPos = _rightPlayer.transform.position + _offSetPos;
        _leftCamera.transform.position = leftPos;
        _rightCamera.transform.position = rightPos;
    }
}
