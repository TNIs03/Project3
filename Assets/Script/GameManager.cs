using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Difficulty selectedDifficulty = Difficulty.Easy;

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
    }

    public void SetDifficulty(int index)
    {
        selectedDifficulty = (Difficulty)index;
        Debug.Log("Difficulty set to: " + selectedDifficulty);
    }
}
