using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float timeRemaining = 30f;  // 30 секунд на игру
    private bool isGameActive = true;
    public IPv4 ipv4;
    void Update()
    {
        if (isGameActive)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = $"{Mathf.Ceil(timeRemaining)}";

            if (timeRemaining <= 0)
            {
                isGameActive = false;
                EndGame();
            }
        }
    }

    void EndGame()
    {
        timerText.text = "Time's Up!";
        foreach (Button button in ipv4.classButtons)
        {
            button.interactable = false;
        }
        ipv4.feedbackText.text = $"Your score is: {ipv4.score}";
        // Тут можно добавить финальный подсчет очков, показать итоговый экран и т.д.
    }
}
