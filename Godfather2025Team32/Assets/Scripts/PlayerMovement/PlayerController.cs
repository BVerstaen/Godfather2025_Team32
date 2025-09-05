using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
 [Header("Spline")]
    public SplineContainer spline;
    public bool loop = true;

    [Header("Speed")]
    public float moveSpeed = 12f;          // m/s de base
    public float acceleration = 4f;        // lerp vers target
    public float speedMultiplier = 1f;     // boost multiplicatif
    public float minSpeedMultiplier = 0.1f;

    [Header("Sub-steps (anti-skip)")]
    [Tooltip("Distance max en mètres par sous-pas de mouvement.")]
    public float maxMetersPerStep = 0.5f;  // plus petit = plus précis
    [Tooltip("Limite de sous-pas par frame (sécurité perf).")]
    public int maxSubStepsPerFrame = 50;

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

    [Header("Team")]
    public Team currentTeam = Team.None;

    // --- internes ---
    private float _pathLength = 0f;          // longueur totale (m)
    private float _distanceOnPath = 0f;      // distance courante (m)
    private float _currentSpeed;             // m/s lissé
    private float _lateralInput = 0f;
    private float _lateralInputTarget = 0f;
    private bool _isStarted = false;

    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
    }

    void Start()
    {
        EventManager.Instance.OnStart += StartMovement;
        EventManager.Instance.OnAccelerate += Accelerate;
        EventManager.Instance.OnDecelerate += Decelerate;

        RebuildLengthCache();
        _currentSpeed = moveSpeed * speedMultiplier;

        // (optionnel) Setup cam selon l’équipe
        if (currentTeam == Team.Team1) CameraManager.Instance.LeftPlayer = gameObject;
        else if (currentTeam == Team.Team2) CameraManager.Instance.RightPlayer = gameObject;
    }

    void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnStart -= StartMovement;
            EventManager.Instance.OnAccelerate -= Accelerate;
            EventManager.Instance.OnDecelerate -= Decelerate;
        }
    }

    public void RebuildLengthCache()
    {
        if (spline == null) { _pathLength = 0f; return; }
        _pathLength = spline.CalculateLength();  // longueur en mètres
        if (drawDebug) Debug.Log($"[Spline] length = {_pathLength:F2} m");
        _distanceOnPath = Mathf.Repeat(_distanceOnPath, Mathf.Max(_pathLength, 0.0001f));
    }

    void Update()
    {
        if (!_isStarted || spline == null || _pathLength <= 0f) return;

        // 1) Vitesse lissée
        float targetSpeed = moveSpeed * Mathf.Max(minSpeedMultiplier, speedMultiplier);
        _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, 1f - Mathf.Exp(-acceleration * Time.deltaTime));

        // 2) Distance à parcourir ce frame
        float frameDist = _currentSpeed * Time.deltaTime;
        if (frameDist <= 0f) { SampleApplyAtDistance(_distanceOnPath); return; }

        // 3) Sous-pas anti-skip
        float step = Mathf.Max(0.001f, Mathf.Min(maxMetersPerStep, frameDist));
        int steps = Mathf.Clamp(Mathf.CeilToInt(frameDist / step), 1, maxSubStepsPerFrame);
        step = frameDist / steps; // réajusté

        // 4) Lateral vers target
        _lateralInput = Mathf.MoveTowards(_lateralInput, _lateralInputTarget, lateralReturnSpeed * Time.deltaTime);

        // 5) Balance (déséquilibre)
        float balance = 0f;
        if (balanceManager) balance = balanceManager.GetImbalance(currentTeam);
        float balanceOffset = balance * lateralRange * balanceEffect;

        // 6) Avance par sous-pas + applique la pose à CHAQUE sous-pas
        for (int i = 0; i < steps; i++)
        {
            _distanceOnPath += step;

            if (loop)
            {
                _distanceOnPath = Mathf.Repeat(_distanceOnPath, _pathLength);
            }
            else
            {
                if (_distanceOnPath >= _pathLength)
                {
                    _distanceOnPath = _pathLength;
                    // on est au bout: on applique et on sort
                    SampleApplyAtDistance(_distanceOnPath, _lateralInput, balanceOffset);
                    break;
                }
            }

            SampleApplyAtDistance(_distanceOnPath, _lateralInput, balanceOffset);
        }
    }

    // Échantillonne la spline à une distance (m) et applique position + rotation
    private void SampleApplyAtDistance(float distMeters, float lateralInput = 0f, float balanceOffset = 0f)
    {
        if (_pathLength <= 0f) return;

        float u = Mathf.Clamp01(distMeters / _pathLength); // 0..1
        Vector3 pos = spline.EvaluatePosition(u);
        float3 tanF3 = spline.EvaluateTangent(u);
        float3 upF3  = spline.EvaluateUpVector(u);

        Vector3 tangent = ((Vector3)math.normalize(tanF3));
        Vector3 up = ((Vector3)math.normalize(upF3));
        Vector3 right = Vector3.Cross(up, tangent).normalized;

        // décalage latéral
        float lateralOffsetValue = (lateralInput * lateralRange) + balanceOffset;
        Vector3 desiredPos = pos + right * lateralOffsetValue;

        // lissage optionnel du recentrage/balance (position uniquement)
        float t = (balanceLerpTime > 0f) ? (1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(0.0001f, balanceLerpTime))) : 1f;
        transform.position = Vector3.Lerp(transform.position, desiredPos, t);

        // orientation
        Quaternion targetRot = Quaternion.LookRotation(tangent, up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 1f - Mathf.Exp(-orientSpeed * Time.deltaTime));

        if (drawDebug)
        {
            Debug.DrawLine(pos, pos + tangent, Color.green);
            Debug.DrawRay(pos, up * 0.5f, Color.blue);
            Debug.DrawRay(pos, right * 0.5f, Color.red);
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