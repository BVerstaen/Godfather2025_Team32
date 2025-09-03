using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonsInputs : MonoBehaviour
{
    [SerializeField] private InputActionAsset[] _allInputActionAssets;
    private InputActionAsset _InputMap;
    private int _playerIndex;
    [SerializeField] private string[] _inputsNames;
    private string _leftSide = "Left";
    private string _rightSide = "Right";

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

        public Dictionary<Buttons, InputAction> dicoActions;

        public InputAction Trigger;
        public InputAction Shoulder;
        public InputAction Up;
        public InputAction Down;
        public InputAction Left;
        public InputAction Right;

        /*public InputActionReference Trigger;
        public InputActionReference Shoulder;
        public InputActionReference Up;
        public InputActionReference Down;
        public InputActionReference Left;
        public InputActionReference Right;*/

        public void SetActions()
        {
            Trigger = dicoActions[Buttons.Trigger];
            Shoulder = dicoActions[Buttons.Shoulder];
            Up = dicoActions[Buttons.Up];
            Down = dicoActions[Buttons.Down];
            Left = dicoActions[Buttons.Left];
            Right = dicoActions[Buttons.Right];
        }
    }

    /*[SerializeField] private PlayerInputs _leftSideInputs;
    [SerializeField] private PlayerInputs _rightSideInputs;*/
    private PlayerInputs _leftSideInputs;
    private PlayerInputs _rightSideInputs;

    public Action<PlayerSide, Buttons> OnButtonPressed;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        GetActionsReferences();

        _leftSideInputs.Trigger.started += LeftTrigger;
        _leftSideInputs.Shoulder.started += LeftShoulder;
        _leftSideInputs.Up.started += LeftUp;
        _leftSideInputs.Down.started += LeftDown;
        _leftSideInputs.Left.started += LeftLeft;
        _leftSideInputs.Right.started += LeftRight;

        _rightSideInputs.Trigger.started += RightTrigger;
        _rightSideInputs.Shoulder.started += RightShoulder;
        _rightSideInputs.Up.started += RightUp;
        _rightSideInputs.Down.started += RightDown;
        _rightSideInputs.Left.started += RightLeft;
        _rightSideInputs.Right.started += RightRight;
    }

    private void OnDisable()
    {
        _leftSideInputs.Trigger.started -= LeftTrigger;
        _leftSideInputs.Shoulder.started -= LeftShoulder;
        _leftSideInputs.Up.started -= LeftUp;
        _leftSideInputs.Down.started -= LeftDown;
        _leftSideInputs.Left.started -= LeftLeft;
        _leftSideInputs.Right.started -= LeftRight;

        _rightSideInputs.Trigger.started -= RightTrigger;
        _rightSideInputs.Shoulder.started -= RightShoulder;
        _rightSideInputs.Up.started -= RightUp;
        _rightSideInputs.Down.started -= RightDown;
        _rightSideInputs.Left.started -= RightLeft;
        _rightSideInputs.Right.started -= RightRight;
    }

    /*private void OnEnable()
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
    }*/

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

    private void GetActionsReferences()
    {
        _playerIndex = GetComponent<PlayerInput>().playerIndex;
        Debug.Log(_playerIndex);
        _InputMap = _allInputActionAssets[_playerIndex];
        GetComponent<PlayerInput>().actions = _InputMap;
        Buttons currentButton;
        int i = 0;
        string inputName;
        PlayerSide currentSide = PlayerSide.Left;
        PlayerInputs currentStruct = new PlayerInputs();
        currentStruct.dicoActions = new Dictionary<Buttons, InputAction>();
        foreach (string actionName in _inputsNames)
        {
            currentButton = (Buttons)i;
            if (currentSide == PlayerSide.Left)
            {
                inputName = _leftSide + actionName;
            }
            else
            {
                inputName = _rightSide + actionName;
            }
            currentStruct.dicoActions[currentButton] = _InputMap.FindAction(inputName);
            i++;
        }

        currentStruct.SetActions();
        _leftSideInputs = currentStruct;
        i = 0;
        currentSide = PlayerSide.Right;
        currentStruct = new PlayerInputs();
        currentStruct.dicoActions = new Dictionary<Buttons, InputAction>();

        foreach (string actionName in _inputsNames)
        {
            currentButton = (Buttons)i;
            if (currentSide == PlayerSide.Left)
            {
                inputName = _leftSide + actionName;
            }
            else
            {
                inputName = _rightSide + actionName;
            }
            currentStruct.dicoActions[currentButton] = _InputMap.FindAction(inputName);
            i++;
        }
        currentStruct.SetActions();
        _rightSideInputs = currentStruct;
    }
}