using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManager : MonoBehaviour
{
    //---------- VARIABLES ----------\\

    /*[SerializeField] private SequenceManager _sequenceManagerTeam1;
    [SerializeField] private SequenceManager _sequenceManagerTeam2;*/
    private List<SequenceManager> _allSequenceManagers = new List<SequenceManager>();
    private List<PlayerInput> _allPlayerInputs = new List<PlayerInput>();
    static private ControllerManager _instance;
    static public ControllerManager Instance { get { return _instance; } private set { _instance = value; } }

    //---------- FUNCTIONS ----------\\

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_instance == null) _instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void AddSequenceManager(SequenceManager manager)
    {
        _allSequenceManagers.Add(manager);
    }

    public void AddPlayerInput(PlayerInput input)
    {
        _allPlayerInputs.Add(input);
    }

    /*private void OnEnable()
    {
        Debug.Log("ControllerManager Enabled");
        PlayerInputManager playerInputManager = GetComponent<PlayerInputManager>();
        playerInputManager.onPlayerJoined += OnPlayerJoin;
    }

    private void OnDisable()
    {
        Debug.Log("ControllerManager Disabled");
        PlayerInputManager playerInputManager = GetComponent<PlayerInputManager>();
        playerInputManager.onPlayerJoined -= OnPlayerJoin;
    }*/

    private void Awake()
    {
        PlayerInputManager playerInputManager = GetComponent<PlayerInputManager>();
        playerInputManager.onPlayerJoined += OnPlayerJoin;
    }

    private void OnPlayerJoin(PlayerInput playerInput)
    {
        Debug.Log("NewPlayerJoined");
        Debug.Log(playerInput.gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}