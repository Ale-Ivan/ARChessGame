using System;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : Singleton<TimerController>
{
    public Slider timerSlider;
    public Text turnText;
    public GameObject quitButton;

    private float full = 25f;
    private float empty = 0f;

    private float activeTime = 0f;

    private readonly float treshold = 0.001f;

    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
        timerSlider.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        activeTime += Time.deltaTime;
        float percent = activeTime / full;

        if (timerSlider.value < (1f - treshold))
        {
            timerSlider.value = Mathf.Lerp(0, 1, percent);
        }
        else
        {
            ARChessGameManager.instance.MakeRandomMove();
            resetTimer(ARChessGameManager.currentPlayer);
        }
    }

    public void DisplayTimer()
    {
        timerSlider.gameObject.SetActive(true);
        quitButton.SetActive(true);
    }

    public void HideTimer()
    {
        timerSlider.gameObject.SetActive(false);
        quitButton.SetActive(false);
    }

    public void startTimer()
    {
        enabled = true;
    }

    public void stopTimer()
    {
        turnText.text = "";
        enabled = false;
    }

    public void resetTimer(string currentPlayer)
    {
        stopTimer();
        turnText.text = currentPlayer + "'s turn";
        timerSlider.value = empty;
        activeTime = 0f;
        startTimer();
    }
}
