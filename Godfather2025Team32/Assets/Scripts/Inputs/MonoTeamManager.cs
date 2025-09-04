using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static ButtonsInputs;
using static CircularMovementDetector;

public class MonoTeamManager : MonoBehaviour
{
    //---------- VARIABLES ----------\\

    [SerializeField] private InputActionAsset _inputActionAssetPrefab;
    [SerializeField] private string[] _inputActionNames;
    [SerializeField] private string[] _sticksInputActionNames;

    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private ButtonsInputs _buttonsInputs;
    [SerializeField] private CircularMovementDetector _sticksInputs;

    [SerializeField] private SequenceManager _sequenceManager;
    public SequenceManager SequenceManager { get { return _sequenceManager; } }

    private InputActionAsset _currentInputActionAsset;

    private int _playerIndex;
    public int PlayerIndex
    {
        get { return _playerIndex; } 
        private set { _playerIndex = value; } 
    }

    public bool HasStarted { get; set; }

    public Team CurrentTeam
    {
        get
        {
            return PlayerIndex == 0 ? Team.Team1 : Team.Team2;
        }
    }


    private string _leftSideName = "Left";
    private string _rightSideName = "Right";

    //---------- FUNCTIONS ----------\\

    private void Awake()
    {
        ControllerManager.Instance.NewMonoTeamManager(this);
    }

    private void OnEnable()
    {
        _buttonsInputs.dicoInputActions = GetButtonsActionsReferences();
        _sticksInputs.dicoSticksActions = GetSticksInputActions();

        //Send SequenceManager to EventManager
        if (CurrentTeam == Team.Team1)
            EventManager.Instance.Team1SequenceManager = SequenceManager;
        else if (CurrentTeam == Team.Team2)
            EventManager.Instance.Team2SequenceManager = SequenceManager;

        EventManager.Instance.OnStart += SetHasStarted;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnStart -= SetHasStarted;
    }

    private void SetHasStarted() => HasStarted = true;


    public Dictionary<PlayerSide, Dictionary<Buttons, InputAction>> GetButtonsActionsReferences()
    {
        // Create a new InputActionAsset from prefab and attach it
        _playerIndex = _playerInput.playerIndex;
        _currentInputActionAsset = Instantiate(_inputActionAssetPrefab);
        _playerInput.actions = _currentInputActionAsset;
        _currentInputActionAsset.Enable();

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

    private Dictionary<PlayerSide, Dictionary<StickDirection, InputAction>> GetSticksInputActions()
    {
        Dictionary<PlayerSide, Dictionary<StickDirection, InputAction>> dicoSticksActions = new Dictionary<PlayerSide, Dictionary<StickDirection, InputAction>>();

        Dictionary<StickDirection, InputAction> dicoLeftSticks = new Dictionary<StickDirection, InputAction>();
        Dictionary<StickDirection, InputAction> dicoRightSticks = new Dictionary<StickDirection, InputAction>();

        string inputName;
        InputAction currentAction;
        StickDirection currentDirection;
        int length = _sticksInputActionNames.Length;
        for (int i = 0; i < length; i++)
        {
            currentDirection = (StickDirection)i;

            inputName = _leftSideName + _sticksInputActionNames[i];
            currentAction = _currentInputActionAsset.FindAction(inputName);
            dicoLeftSticks[currentDirection] = currentAction;

            inputName = _rightSideName + _sticksInputActionNames[i];
            currentAction = _currentInputActionAsset.FindAction(inputName);
            dicoRightSticks[currentDirection] = currentAction;
        }

        dicoSticksActions[PlayerSide.Left] = dicoLeftSticks;
        dicoSticksActions[PlayerSide.Right] = dicoRightSticks;

        return dicoSticksActions;
    }
}