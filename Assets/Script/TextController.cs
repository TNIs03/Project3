using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    [SerializeField] GameObject PointText;
    [SerializeField] GameObject MoveText;
    public static TextController Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
}