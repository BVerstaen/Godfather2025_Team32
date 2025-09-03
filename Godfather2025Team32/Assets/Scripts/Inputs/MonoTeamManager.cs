using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static ButtonsInputs;

public class MonoTeamManager : MonoBehaviour
{
    //---------- VARIABLES ----------\\

    //[SerializeField] private InputActionAsset _inputActionAssetPrefab;
    [SerializeField] private InputActionAsset[] _arrayInputActionAssets;
    [SerializeField] private string[] _inputActionNames;

    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private ButtonsInputs _buttonsInputs;

    private InputActionAsset _currentInputActionAsset;

    private int _playerIndex;
    public int PlayerIndex
    {
        get { return _playerIndex; } 
        private set { _playerIndex = value; } 
    }

    private string _leftSideName = "Left";
    private string _rightSideName = "Right";

    //---------- FUNCTIONS ----------\\

    private void OnEnable()
    {
        Debug.Log("EnableTeam");
        //_buttonsInputs.dicoInputActions = GetButtonsActionsReferences();
    }

    public Dictionary<PlayerSide, Dictionary<Buttons, InputAction>> GetButtonsActionsReferences()
    {
        // Create a new InputActionAsset from prefab and attach it
        _playerIndex = _playerInput.playerIndex;
        
        _currentInputActionAsset = _arrayInputActionAssets[_playerIndex];
        _playerInput.actions = _currentInputActionAsset;
        /*_currentInputActionAsset = Instantiate(_inputActionAssetPrefab);
        _playerInput.actions = _currentInputActionAsset;
        Debug.Log(_currentInputActionAsset);*/

        // Set up variables for loop for each Actions
        Buttons currentButton;
        int length = _inputActionNames.Length;
        string inputName;
        Dictionary<PlayerSide, Dictionary<Buttons, InputAction>> dicoInputActions = new Dictionary<PlayerSide, Dictionary<Buttons, InputAction>>();
        Dictionary<Buttons, InputAction> dicoInputActionsLeft = new Dictionary<Buttons, InputAction>();
        Dictionary<Buttons, InputAction> dicoInputActionsRight = new Dictionary<Buttons, InputAction>();
        InputAction currentAction;

        for (int i = 0; i < length; i++)
        {
            currentButton = (Buttons)i;
            
            // Left sides buttons
            inputName = _leftSideName + _inputActionNames[i];
            currentAction = _currentInputActionAsset.FindAction(inputName);
            dicoInputActionsLeft[currentButton] = currentAction;

            // Right sides buttons
            inputName = _rightSideName + _inputActionNames[i];
            currentAction = _currentInputActionAsset.FindAction(inputName);
            dicoInputActionsRight[currentButton] = currentAction;
        }
        
        dicoInputActions[PlayerSide.Left] = dicoInputActionsLeft;
        dicoInputActions[PlayerSide.Right] = dicoInputActionsRight;

        return dicoInputActions;
    }
}
