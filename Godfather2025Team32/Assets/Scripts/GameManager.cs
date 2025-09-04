using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Rounds")]
    [SerializeField] private int _numberOfRounds;
    public string podiumSceneName = "PodiumScene";
    public string gameSceneName = "GameScene";

    private bool _hasGameEnded = true;
    private int _currentRound;
    private int _pointsTeam1;
    private int _pointsTeam2;

    public bool HasGameEnded { get => _hasGameEnded; set => _hasGameEnded = value; }

    public Team WinnerTeam { get { return (_pointsTeam1 > _pointsTeam2) ? Team.Team1 : Team.Team2; } }

    public int CurrentRound { get => _currentRound; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ResetGame()
    {
        if (!_hasGameEnded)
            return;

        _currentRound = 0;
        _pointsTeam1 = 0;
        _pointsTeam2 = 0;
        _hasGameEnded = false;
    }

    public void SetRoundWinner(Team team)
    {
        if (team == Team.Team1)
            _pointsTeam1++;
        else if (team == Team.Team2)
            _pointsTeam2++;

        _currentRound++;

        if(_currentRound == _numberOfRounds)
        {
            SceneTransitionUI.Instance.LoadSceneWithTransition(podiumSceneName);
        }
        else
        {
            SceneTransitionUI.Instance.LoadSceneWithTransition(gameSceneName);
        }
    }
}
