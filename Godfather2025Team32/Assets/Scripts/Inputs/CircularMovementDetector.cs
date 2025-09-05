using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static ButtonsInputs;

public class CircularMovementDetector : MonoBehaviour
{
    public Dictionary<PlayerSide, Dictionary<StickDirection, InputAction>> dicoSticksActions;

    public enum StickDirection { Horizontal, Vertical }

    public enum RotationDirection
    {
        Clockwise = -1,
        CounterClockwise = 1
    }

    public enum StickType
    {
        LeftStick,
        RightStick
    }

    private class CircularStickData
    {
        public StickType Type;

        public Vector2 PreviousDirection;
        public float PreviousAngle = 0;
        public float TotalAngle = 0;
        public RotationDirection CurrentDirection;
    }

    private CircularStickData _leftStickData = new CircularStickData();
    private CircularStickData _rightStickData = new CircularStickData();

    public Action<StickType, RotationDirection> OnDetectCircularMovement;

    private void Awake()
    {
        _leftStickData.Type = StickType.LeftStick;
        _rightStickData.Type = StickType.RightStick;
    }

    void Update()
    {
        CalculateCircularMovement(_leftStickData);
        CalculateCircularMovement(_rightStickData);
    }


    private void CalculateCircularMovement(CircularStickData stickData)
    {
        float x, y;
        if (stickData.Type == StickType.LeftStick)
        {
            x = dicoSticksActions[PlayerSide.Left][StickDirection.Horizontal].ReadValue<float>();
            y = dicoSticksActions[PlayerSide.Left][StickDirection.Vertical].ReadValue<float>();
        }
        else
        {
            x = dicoSticksActions[PlayerSide.Right][StickDirection.Horizontal].ReadValue<float>();
            y = dicoSticksActions[PlayerSide.Right][StickDirection.Vertical].ReadValue<float>();
        }
        Vector2 input = new Vector2(x, y);

        if (input.magnitude > 0.2f) // seuil pour éviter le bruit
        {
            float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

            // Calcul du delta angle
            float deltaAngle = Mathf.DeltaAngle(stickData.PreviousAngle, angle);
            int deltaSign = (int)Mathf.Sign(deltaAngle);

            // Détection de direction de rotation
            if (Mathf.Abs(deltaAngle) > 1f && Mathf.Abs(deltaAngle) < 60f)
            {
                if (stickData.CurrentDirection == 0)
                {
                    stickData.CurrentDirection = (deltaSign > 0) ? RotationDirection.CounterClockwise : RotationDirection.Clockwise;
                }

                if (deltaSign == (int)stickData.CurrentDirection)
                {
                    stickData.TotalAngle += Mathf.Abs(deltaAngle);
                }
                else if (deltaSign != 0)
                {
                    //Debug.Log("Inversé !");
                    stickData.TotalAngle = 0;
                    stickData.CurrentDirection = 0;
                }

                if (stickData.TotalAngle >= 360f)
                {
                    //Debug.Log("Mouvement circulaire détecté !");
                    //Debug.Log($"{stickData.Type} - {stickData.CurrentDirection}");
                    OnDetectCircularMovement?.Invoke(stickData.Type, stickData.CurrentDirection);
                    stickData.TotalAngle = 0;
                    stickData.CurrentDirection = 0;
                }
            }

            stickData.PreviousAngle = angle;
        }
        else
        {
            // Joystick au repos : reset
            stickData.TotalAngle = 0;
            stickData.CurrentDirection = 0;
        }
    }
}
