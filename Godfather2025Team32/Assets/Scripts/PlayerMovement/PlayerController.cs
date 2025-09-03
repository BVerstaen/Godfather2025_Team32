using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SkiController : MonoBehaviour
{
    [Header("Movement")]
    public float baseSpeed = 12f;
    public float speedMultiplier = 1f;
    public float acceleration = 4f;
    public float lateralSpeed = 4f;
    public float maxLateralInput = 1f;

    [Header("Ground / Slope")]
    public float groundCheckDistance = 1.2f;
    public LayerMask groundMask = ~0;
    public float stickToGroundForce = 5f;

    [Header("Orientation")]
    public float orientSpeed = 8f;
    public float forwardLookSpeed = 8f;

    [Header("Debug / Helpers")]
    public bool drawDebug = false;

    Rigidbody rb;
    float currentSpeed;
    float lateralInput = 0f;
    Vector3 lastGroundNormal = Vector3.up;
    bool grounded = false;

    private bool isStarted = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        currentSpeed = 0;

        StartMovement();
    }

    void FixedUpdate()
    {
        if (isStarted)
        {
            // Ground check
            RaycastHit hit;
            Vector3 rayStart = transform.position + Vector3.up * 0.2f;
            grounded = Physics.Raycast(rayStart, Vector3.down, out hit, groundCheckDistance, groundMask);
            Vector3 groundNormal = grounded ? hit.normal : Vector3.up;
            lastGroundNormal = groundNormal;

            // Descente
            Vector3 downhill = Vector3.ProjectOnPlane(Vector3.down, groundNormal).normalized;
            if (downhill.sqrMagnitude < 0.0001f)
                downhill = Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized;

            Vector3 right = Vector3.Cross(downhill, groundNormal).normalized;

            // Calcul de vitesse
            float targetSpeed = baseSpeed * speedMultiplier;
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, 1f - Mathf.Exp(-acceleration * Time.fixedDeltaTime));

            Vector3 forwardVel = downhill * currentSpeed;
            Vector3 lateralVel = right * lateralInput * lateralSpeed;

            Vector3 targetVel = forwardVel + lateralVel;
            Vector3 normalComp = Vector3.Project(rb.linearVelocity, groundNormal);

            rb.linearVelocity = targetVel + normalComp;

            if (!grounded)
                rb.linearVelocity += Physics.gravity * Time.fixedDeltaTime;
            else
                rb.AddForce(-groundNormal * stickToGroundForce * Time.fixedDeltaTime, ForceMode.VelocityChange);

            OrientToSlope(groundNormal, downhill);   
        }
    }

    void OrientToSlope(Vector3 groundNormal, Vector3 downhill)
    {
        Vector3 newUp = Vector3.Slerp(transform.up, groundNormal, 1f - Mathf.Exp(-orientSpeed * Time.deltaTime));
        transform.up = newUp;

        Vector3 horizontalVel = rb.linearVelocity - Vector3.Project(rb.linearVelocity, transform.up);
        if (horizontalVel.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(horizontalVel.normalized, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 1f - Mathf.Exp(-forwardLookSpeed * Time.deltaTime));
        }
        else if (downhill.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(downhill, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 1f - Mathf.Exp(-forwardLookSpeed * Time.deltaTime));
        }
    }

    #region Public API (événements)

    public void StartMovement()
    {
        if (speedMultiplier <= 0f)
            speedMultiplier = 1f;

        currentSpeed = baseSpeed * speedMultiplier;
        isStarted = true;
    }
    
    public void Accelerate(float amount)
    {
        speedMultiplier += Mathf.Max(0f, amount);
    }

    public void Decelerate(float amount)
    {
        speedMultiplier -= Mathf.Max(0f, amount);
        speedMultiplier = Mathf.Max(0f, speedMultiplier);
    }

    public void MoveLeft(float amount)
    {
        lateralInput = Mathf.Clamp(lateralInput - amount, -maxLateralInput, maxLateralInput);
    }

    public void MoveRight(float amount)
    {
        lateralInput = Mathf.Clamp(lateralInput + amount, -maxLateralInput, maxLateralInput);
    }

    public void SetLateralInput(float t)
    {
        lateralInput = Mathf.Clamp(t, -maxLateralInput, maxLateralInput);
    }

    public void AddLateralInput(float delta)
    {
        SetLateralInput(lateralInput + delta);
    }

    public bool IsGrounded() => grounded;

    #endregion

    void OnDrawGizmos()
    {
        if (!drawDebug) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.2f, Vector3.down * groundCheckDistance);
    }
}