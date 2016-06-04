using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartingGameTextManager : MonoBehaviour {
    public Text startingText;
    public int waitTime;

    private float currentWait;

    void Awake()
    {
        currentWait = waitTime;
        setText();
    }

    void Update()
    {
        currentWait -= Time.deltaTime;
        setText();
    }

    private void setText()
    {
        startingText.text = "Starting game in\n" + (int) currentWait;
    }
}
