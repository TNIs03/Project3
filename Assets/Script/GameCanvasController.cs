using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvasController : MonoBehaviour
{
    [SerializeField] GameObject PointText;
    [SerializeField] GameObject MoveText;
    public static GameCanvasController Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdatePoint(int newPoint)
    {
        PointText.GetComponent<Text>().text = "POINTS: " + newPoint;
    }
    public void UpdateMove(int newMove)
    {
        MoveText.GetComponent<Text>().text = "MOVES LEFT: " + newMove;
    }
    public void OnPauseClicked()
    {
        StartCoroutine(PauseGame());
    }
    private IEnumerator PauseGame()
    {
        DialogController.Instance.OnGamePause();
        while (GameManager.GetGameState() == GameState.Animating)
        {
            yield return new WaitForSeconds(0.1f);
        }
        GameManager.SetGameState(GameState.Pausing);
    }
}