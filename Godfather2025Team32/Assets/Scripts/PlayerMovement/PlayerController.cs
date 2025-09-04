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
    public float speedMultiplier = 1f;
    public float minSpeedMultiplier = 0.1f;

    [Header("Lateral")]
    public float lateralRange = 2f;
    public float maxLateralInput = 1f;
    public float lateralReturnSpeed = 1f;
    
    [Header("Balance (déséquilibre)")]
    public BalanceManager balanceManager;
    public float balanceEffect = 1f;
    public float balanceLerpTime = 0.12f;

    [Header("Orientation")]
    public float orientSpeed = 8f;

    [Header("Debug / Helpers")]
    public bool drawDebug = false;

    [Header("Events")]
    public EventManager eventManager;
    public Team currentTeam = Team.None;

    private float _splinePos = 0f;
    private float _lateralInput = 0f;
    private float _lateralInputTarget = 0f;
    private float _currentSpeed;
    private bool _isStarted = false;

    private float _cachedLength = 0f;
    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true; // on suit la spline, pas la physique (changer si besoin)
    }

    void Start()
    {
        if (eventManager != null)
        {
            eventManager.OnStart += StartMovement;
            eventManager.OnAccelerate += Accelerate;
        }

        if (spline != null) 
            RebuildLengthCache();
        
        _currentSpeed = moveSpeed * speedMultiplier;

        //Setup cam
        if (currentTeam == Team.Team1)
            CameraManager.Instance.LeftPlayer = gameObject;
        else if (currentTeam == Team.Team2)
            CameraManager.Instance.RightPlayer = gameObject;
    }

    void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.OnStart -= StartMovement;
            eventManager.OnAccelerate -= Accelerate;
        }
    }

    public void RebuildLengthCache()
    {
        if (spline == null) { _cachedLength = 0f; return; }
        _cachedLength = spline.CalculateLength();
        if (drawDebug) Debug.Log($"[Spline] cachedLength = {_cachedLength}");
    }

    void Update()
    {
        if (!_isStarted || spline == null)
            return;

        if (_cachedLength <= 0f)
        {
            if (drawDebug) Debug.LogWarning("[PlayerController] spline length is <= 0. RebuildLengthCache() or check your spline points.");
            return;
        }

        float targetSpeed = moveSpeed * Mathf.Max(minSpeedMultiplier, speedMultiplier);
        _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, 1f - Mathf.Exp(-acceleration * Time.deltaTime));

        float deltaNormalized = (_currentSpeed * Time.deltaTime) / _cachedLength;
        _splinePos += deltaNormalized;
        
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

        _lateralInput = Mathf.MoveTowards(_lateralInput, _lateralInputTarget, lateralReturnSpeed * Time.deltaTime);

        float balance = 0f;
        if (balanceManager)
            balance = balanceManager.GetImbalance(currentTeam);

        float balanceOffset = balance * lateralRange * balanceEffect;
        float lateralOffsetValue = (_lateralInput * lateralRange) + balanceOffset;

        Vector3 targetOffset = right * lateralOffsetValue;
        Vector3 desiredPos = pos + targetOffset;

        float t = (balanceLerpTime > 0f) ? (1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(0.0001f, balanceLerpTime))) : 1f;
        transform.position = Vector3.Lerp(transform.position, desiredPos, t);

        Quaternion targetRot = Quaternion.LookRotation(tangent, up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 1f - Mathf.Exp(-orientSpeed * Time.deltaTime));

        if (drawDebug)
        {
            Debug.DrawLine(pos, pos + tangent, Color.green);
            Debug.DrawRay(pos, up * 0.5f, Color.blue);
            Debug.DrawRay(pos, right * 0.5f, Color.red);
            Debug.Log($"deltaNormalized={deltaNormalized:F5} splinePos={_splinePos:F5} lateralIn={_lateralInput:F2} target={_lateralInputTarget:F2} balance={balance:F2}");
        }
    }

    #region Public API (events)

    public void StartMovement()
    {
        _isStarted = true;
    }

    /// <summary>Accélère multiplicativement (amount = 0.2 => +20%)</summary>
    public void Accelerate(Team team, float amount)
    {
        if (team != currentTeam) return;
        float factor = 1f + Mathf.Max(-0.99f, amount);
        speedMultiplier *= factor;
    }

    /// <summary>Décélère multiplicativement (amount = 0.2 => /1.2)</summary>
    public void Decelerate(Team team, float amount)
    {
        if (team != currentTeam) return;
        float factor = 1f + Mathf.Max(0f, amount);
        speedMultiplier = Mathf.Max(minSpeedMultiplier, speedMultiplier / factor);
    }

    /// <summary>Appelé par l'EventManager/SequenceManager : ajoute une poussée latérale target</summary>
    public void MoveLeft(Team team, float amount)
    {
        if (team != currentTeam) return;
        _lateralInputTarget = Mathf.Clamp(_lateralInputTarget - amount, -maxLateralInput, maxLateralInput);
    }

    public void MoveRight(Team team, float amount)
    {
        if (team != currentTeam) return;
        _lateralInputTarget = Mathf.Clamp(_lateralInputTarget + amount, -maxLateralInput, maxLateralInput);
    }

    public void SetLateralInput(float t)
    {
        _lateralInputTarget = Mathf.Clamp(t, -maxLateralInput, maxLateralInput);
    }

    /// <summary>Ajouter un delta à la target</summary>
    public void AddLateralInput(float delta)
    {
        SetLateralInput(_lateralInputTarget + delta);
    }

    #endregion
}