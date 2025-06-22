using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogController : MonoBehaviour
{
    [SerializeField] GameObject textObject;
    [SerializeField] GameObject closeButton;
    [SerializeField] GameObject title;
    public static DialogController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void OnGameOver(string text)
    {
        gameObject.SetActive(true);
        closeButton.SetActive(false);
        textObject.GetComponent<Text>().text = text;
        title.GetComponentInChildren<Text>().text = "GAME OVER";
    }
    public void OnGamePause()
    {
        gameObject.SetActive(true);
        closeButton.SetActive(true);
        textObject.GetComponent<Text>().text = "GAME PAUSED";
        title.GetComponentInChildren<Text>().text = "PAUSE";
    }
    public void BackToMainMenu()
    {
        GameManager.SetGameState(GameState.MainMenu);
        SceneManager.LoadScene("MenuScene");
    }
    public void OnCloseClicked()
    {
        GameManager.SetGameState(GameState.PLaying);
        gameObject.SetActive(false);
    }
}
