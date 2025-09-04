using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Spline")]
    public SplineContainer spline;
    public float moveSpeed = 12f;
    public float acceleration = 4f;
    public bool loop = true;
    
    [Header("Speed Multiplier (acceleration)")]
    [Tooltip("Multiplicateur appliqué à moveSpeed. Ex: 1 = normal, 1.2 = +20%")]
    public float speedMultiplier = 1f;
    public float minSpeedMultiplier = 0.1f;

    [Header("Lateral")]
    public float lateralRange = 2f;
    public float maxLateralInput = 1f;

    [Header("Orientation")]
    public float orientSpeed = 8f;

    [Header("Debug / Helpers")]
    public bool drawDebug = false;

    [Header("Events")]
    public EventManager eventManager;
    public Team currentTeam = Team.None;

    private float _splinePos = 0f;
    private float _lateralInput = 0f;
    private float _currentSpeed;
    private bool _isStarted = false;

    private float _cachedLength = 0f;
    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
    }

    void Start()
    {
        if (eventManager != null)
        {
            eventManager.OnStart += StartMovement;
            eventManager.OnAccelerate += Accelerate;
            eventManager.OnMoveLeft += MoveLeft;
            eventManager.OnMoveRight += MoveRight;
        }

        if (spline != null) 
            RebuildLengthCache();
        
        _currentSpeed = moveSpeed * speedMultiplier;

        //Setup cam
        if (currentTeam == Team.Team1)
            CameraManager.Instance.LeftPlayer = gameObject;
        else if (currentTeam == Team.Team2)
            CameraManager.Instance.RightPlayer = gameObject;

        StartMovement();
    }

    void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.OnStart -= StartMovement;
            eventManager.OnAccelerate -= Accelerate;
            eventManager.OnMoveLeft -= MoveLeft;
            eventManager.OnMoveRight -= MoveRight;
        }
    }

    public void RebuildLengthCache()
    {
        if (spline == null)
        {
            _cachedLength = 0f; 
            return;
        }
        
        _cachedLength = spline.CalculateLength();
        if (drawDebug)
            Debug.Log($"[Spline] cachedLength = {_cachedLength}");
    }

    void Update()
    {
        if (!_isStarted || spline == null) return;

        if (_cachedLength <= 0f)
        {
            if (drawDebug) Debug.LogWarning("[PlayerController] spline length is <= 0. RebuildLengthCache() or check your spline points.");
            return;
        }

        float targetSpeed = moveSpeed * Mathf.Max(minSpeedMultiplier, speedMultiplier);
        _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, 1f - Mathf.Exp(-acceleration * Time.deltaTime));

        float deltaNormalized = (_currentSpeed * Time.deltaTime) /_cachedLength;
        _splinePos = _splinePos + deltaNormalized;

        if (loop)
            _splinePos = Mathf.Repeat(_splinePos, 1f);
        else
            _splinePos = Mathf.Clamp01(_splinePos);

        Vector3 pos = spline.EvaluatePosition(_splinePos);
        float3 tanF3 = spline.EvaluateTangent(_splinePos);
        float3 upF3 = spline.EvaluateUpVector(_splinePos);

        Vector3 tangent = (Vector3)math.normalize(tanF3);
        Vector3 up = (Vector3)math.normalize(upF3);

        Vector3 right = Vector3.Cross(up, tangent).normalized;
        Vector3 offset = right * (_lateralInput * lateralRange);

        // appliquer position + rotation
        transform.position = pos + offset;
        Quaternion targetRot = Quaternion.LookRotation(tangent, up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 1f - Mathf.Exp(-orientSpeed * Time.deltaTime));

        if (drawDebug)
        {
            Debug.DrawLine(pos, pos + tangent, Color.green);
            Debug.DrawRay(pos, up * 0.5f, Color.blue);
            Debug.Log($"deltaNormalized={deltaNormalized:F5} splinePos={_splinePos:F5}");
        }
    }

    #region Public API (events)

    public void StartMovement()
    {
        _isStarted = true;
    }

    public void Accelerate(Team team, float amount)
    {
        if (team != currentTeam) 
            return;
        
        float factor = 1f + Mathf.Max(-0.99f, amount);
        speedMultiplier *= factor;
    }

    public void Decelerate(Team team, float amount)
    {
        if (team != currentTeam) 
            return;
        
        float factor = 1f + Mathf.Max(0f, amount);
        speedMultiplier = Mathf.Max(minSpeedMultiplier, speedMultiplier / factor);
    }

    public void MoveLeft(Team team, float amount)
    {
        if (team != currentTeam)
            return;
        
        _lateralInput = Mathf.Clamp(_lateralInput - amount, -maxLateralInput, maxLateralInput);
    }

    public void MoveRight(Team team, float amount)
    {
        if (team != currentTeam) return;
        _lateralInput = Mathf.Clamp(_lateralInput + amount, -maxLateralInput, maxLateralInput);
    }

    public void SetLateralInput(float t)
    {
        _lateralInput = Mathf.Clamp(t, -maxLateralInput, maxLateralInput);
    }

    public void AddLateralInput(float delta)
    {
        SetLateralInput(_lateralInput + delta);
    }

    #endregion
}