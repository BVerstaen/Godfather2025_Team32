using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonsInputs : MonoBehaviour
{
    public enum PlayerSide { Left, Right };

    public enum Buttons
    {
        Trigger,
        Shoulder,
        Up,
        Down,
        Left,
        Right,
    }

    public Dictionary<PlayerSide, Dictionary<Buttons, InputAction>> dicoInputActions;
    public Action<PlayerSide, Buttons> OnButtonPressed;

    private void OnEnable()
    {
        dicoInputActions[PlayerSide.Left][Buttons.Trigger].started += LeftTrigger;
        dicoInputActions[PlayerSide.Left][Buttons.Shoulder].started += LeftShoulder;
        dicoInputActions[PlayerSide.Left][Buttons.Up].started += LeftUp;
        dicoInputActions[PlayerSide.Left][Buttons.Down].started += LeftDown;
        dicoInputActions[PlayerSide.Left][Buttons.Left].started += LeftLeft;
        dicoInputActions[PlayerSide.Left][Buttons.Right].started += LeftRight;

        dicoInputActions[PlayerSide.Right][Buttons.Trigger].started += RightTrigger;
        dicoInputActions[PlayerSide.Right][Buttons.Shoulder].started += RightShoulder;
        dicoInputActions[PlayerSide.Right][Buttons.Up].started += RightUp;
        dicoInputActions[PlayerSide.Right][Buttons.Down].started += RightDown;
        dicoInputActions[PlayerSide.Right][Buttons.Left].started += RightLeft;
        dicoInputActions[PlayerSide.Right][Buttons.Right].started += RightRight;
    }

    private void OnDisable()
    {
        dicoInputActions[PlayerSide.Left][Buttons.Trigger].started -= LeftTrigger;
        dicoInputActions[PlayerSide.Left][Buttons.Shoulder].started -= LeftShoulder;
        dicoInputActions[PlayerSide.Left][Buttons.Up].started -= LeftUp;
        dicoInputActions[PlayerSide.Left][Buttons.Down].started -= LeftDown;
        dicoInputActions[PlayerSide.Left][Buttons.Left].started -= LeftLeft;
        dicoInputActions[PlayerSide.Left][Buttons.Right].started -= LeftRight;

        dicoInputActions[PlayerSide.Right][Buttons.Trigger].started -= RightTrigger;
        dicoInputActions[PlayerSide.Right][Buttons.Shoulder].started -= RightShoulder;
        dicoInputActions[PlayerSide.Right][Buttons.Up].started -= RightUp;
        dicoInputActions[PlayerSide.Right][Buttons.Down].started -= RightDown;
        dicoInputActions[PlayerSide.Right][Buttons.Left].started -= RightLeft;
        dicoInputActions[PlayerSide.Right][Buttons.Right].started -= RightRight;
    }
    private void RightRight(InputAction.CallbackContext context) => OnButtonPressed(PlayerSide.Right, Buttons.Right);

    private void RightLeft(InputAction.CallbackContext context) => OnButtonPressed(PlayerSide.Right, Buttons.Left);

    private void RightDown(InputAction.CallbackContext context) => OnButtonPressed(PlayerSide.Right, Buttons.Down);

    private void RightUp(InputAction.CallbackContext context) => OnButtonPressed(PlayerSide.Right, Buttons.Up);

    private void RightShoulder(InputAction.CallbackContext context) => OnButtonPressed(PlayerSide.Right, Buttons.Shoulder);

    private void RightTrigger(InputAction.CallbackContext context) => OnButtonPressed(PlayerSide.Right, Buttons.Trigger);

    private void LeftRight(InputAction.CallbackContext context) => OnButtonPressed(PlayerSide.Left, Buttons.Right);

    private void LeftDown(InputAction.CallbackContext context) => OnButtonPressed(PlayerSide.Left, Buttons.Down);

    private void LeftLeft(InputAction.CallbackContext context) => OnButtonPressed(PlayerSide.Left, Buttons.Left);

    private void LeftUp(InputAction.CallbackContext context) => OnButtonPressed(PlayerSide.Left, Buttons.Up);

    private void LeftShoulder(InputAction.CallbackContext context) => OnButtonPressed(PlayerSide.Left, Buttons.Shoulder);

    private void LeftTrigger(InputAction.CallbackContext context) => OnButtonPressed(PlayerSide.Left, Buttons.Trigger);
}