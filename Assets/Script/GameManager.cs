using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Difficulty selectedDifficulty = Difficulty.Easy;
    private GameState gameState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keep across scenes
        }
        else
        {
            Destroy(gameObject);
        }
        gameState = GameState.MainMenu;
    }

    public void SetDifficulty(int index)
    {
        selectedDifficulty = (Difficulty)index;
        Debug.Log("Difficulty set to: " + selectedDifficulty);
    }

    public static void SetGameState(GameState gameState)
    {
        Instance.gameState = gameState;
    }

    public static GameState GetGameState()
    {
        return Instance.gameState;
    }
}
