using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogController : MonoBehaviour
{
    [SerializeField] GameObject textObject;
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
        textObject.GetComponent<Text>().text = text;
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
