using UnityEngine;
using UnityEngine.InputSystem;

public class CircularMovementDetector : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference _horizontalInput;
    [SerializeField] private InputActionReference _verticalInput;

    public enum RotationDirection
    {
        Clockwise = -1,
        CounterClockwise = 1
    }

    private Vector2 _previousDirection;
    private float _previousAngle = 0f;
    private float _totalAngle = 0f;
    private RotationDirection _currentRotationDirection;

    void Update()
    {
        float x = _horizontalInput.action.ReadValue<float>();
        float y = _verticalInput.action.ReadValue<float>();
        Vector2 input = new Vector2(x, y);

        if (input.magnitude > 0.2f) // seuil pour éviter le bruit
        {
            float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

            // Calcul du delta angle
            float deltaAngle = Mathf.DeltaAngle(_previousAngle, angle);
            int deltaSign = (int)Mathf.Sign(deltaAngle);

            // Détection de direction de rotation
            if (Mathf.Abs(deltaAngle) > 1f && Mathf.Abs(deltaAngle) < 60f)
            {
                if (_currentRotationDirection == 0)
                {
                    _currentRotationDirection = (deltaSign > 0) ? RotationDirection.CounterClockwise : RotationDirection.Clockwise;
                    Debug.Log($"Détection du sens : {_currentRotationDirection}");
                }

                if (deltaSign == (int)_currentRotationDirection)
                {
                    _totalAngle += Mathf.Abs(deltaAngle);
                }
                else if (deltaSign != 0)
                {
                    Debug.Log("Inversé !");
                    _totalAngle = 0;
                    _currentRotationDirection = 0;
                }

                if (_totalAngle >= 360f)
                {
                    Debug.Log("Mouvement circulaire détecté !");
                    _totalAngle = 0;
                    _currentRotationDirection = 0;
                }
            }

            _previousAngle = angle;
        }
        else
        {
            // Joystick au repos : reset
            _totalAngle = 0;
            _currentRotationDirection = 0;
        }
    }

}
