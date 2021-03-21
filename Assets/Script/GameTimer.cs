using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public float timeElapsed = 0f;
    public Text timeText;
    public bool timerIsActive;
    // Start is called before the first frame update
    void Start()
    {
        timerIsActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerIsActive)
        {
            timeElapsed += Time.deltaTime;
            float minutes = Mathf.FloorToInt(timeElapsed / 60);
            float seconds = Mathf.FloorToInt(timeElapsed % 60);
            DisplayTime(timeElapsed);
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        float milliSeconds = (timeToDisplay % 1) * 100;

        timeText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliSeconds);
    }

    public void StartTimer()
    {
        if (!timerIsActive)
        {
            timerIsActive = true;
            timeElapsed = 0;
        }
    }

    public void StopTimer()
    {
        if (timerIsActive)
        {
            timerIsActive = false;
        }
    }
}
