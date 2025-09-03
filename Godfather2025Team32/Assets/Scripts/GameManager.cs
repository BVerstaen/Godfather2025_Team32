using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Team WinnerTeam { get; private set; } = Team.None;

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

    public void SetWinner(Team team)
    {
        WinnerTeam = team;
    }
}
