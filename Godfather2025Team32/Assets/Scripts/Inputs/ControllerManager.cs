using UnityEngine;
using System.Collections.Generic;

public class ControllerManager : MonoBehaviour
{
    //---------- VARIABLES ----------\\
    static private ControllerManager _instance;
    static public ControllerManager Instance {  get { return _instance; } }

    private List<MonoTeamManager> _listTeamManagers = new List<MonoTeamManager>();
    public List<MonoTeamManager> ListTeamManagers {  get { return _listTeamManagers; } }

    //---------- FUNCTIONS ----------\\

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void NewMonoTeamManager(MonoTeamManager newManager)
    {
        _listTeamManagers.Add(newManager);
    }
}