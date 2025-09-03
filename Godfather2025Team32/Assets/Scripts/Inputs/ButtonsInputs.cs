using System;
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

    [System.Serializable]
    private struct PlayerInputs
    {
        public PlayerSide side;

        public InputActionReference Trigger;
        public InputActionReference Shoulder;
        public InputActionReference Up;
        public InputActionReference Down;
        public InputActionReference Left;
        public InputActionReference Right;
    }

    [SerializeField] private PlayerInputs _leftSideInputs;
    [SerializeField] private PlayerInputs _rightSideInputs;

    public Action<PlayerSide, Buttons> OnButtonPressed;

    private void OnEnable()
    {
        _leftSideInputs.Trigger.action.started += LeftTrigger;
        _leftSideInputs.Shoulder.action.started += LeftShoulder;
        _leftSideInputs.Up.action.started += LeftUp;
        _leftSideInputs.Down.action.started += LeftDown;
        _leftSideInputs.Left.action.started += LeftLeft;
        _leftSideInputs.Right.action.started += LeftRight;

        _rightSideInputs.Trigger.action.started += RightTrigger;
        _rightSideInputs.Shoulder.action.started += RightShoulder;
        _rightSideInputs.Up.action.started += RightUp;
        _rightSideInputs.Down.action.started += RightDown;
        _rightSideInputs.Left.action.started += RightLeft;
        _rightSideInputs.Right.action.started += RightRight;
    }

    private void OnDisable()
    {
        _leftSideInputs.Trigger.action.started -= LeftTrigger;
        _leftSideInputs.Shoulder.action.started -= LeftShoulder;
        _leftSideInputs.Up.action.started -= LeftUp;
        _leftSideInputs.Down.action.started -= LeftDown;
        _leftSideInputs.Left.action.started -= LeftLeft;
        _leftSideInputs.Right.action.started -= LeftRight;

        _rightSideInputs.Trigger.action.started -= RightTrigger;
        _rightSideInputs.Shoulder.action.started -= RightShoulder;
        _rightSideInputs.Up.action.started -= RightUp;
        _rightSideInputs.Down.action.started -= RightDown;
        _rightSideInputs.Left.action.started -= RightLeft;
        _rightSideInputs.Right.action.started -= RightRight;
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
