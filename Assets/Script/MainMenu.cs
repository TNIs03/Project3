using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject DiffText;
    private int DiffIndex = 0;
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene"); 
    }

    public void OnLeftClick()
    {
        if (DiffIndex == 0) DiffIndex = Enum.GetValues(typeof(Difficulty)).Length - 1;
        else DiffIndex--;
        GameManager.Instance.SetDifficulty(DiffIndex);
        DiffText.GetComponent<Text>().text = ((Difficulty)DiffIndex).ToString();
    }
    public void OnRightClick()
    {
        if (DiffIndex == Enum.GetValues(typeof(Difficulty)).Length - 1) DiffIndex = 0;
        else DiffIndex++;
        GameManager.Instance.SetDifficulty(DiffIndex);
        DiffText.GetComponent<Text>().text = ((Difficulty)DiffIndex).ToString();
    }
}
