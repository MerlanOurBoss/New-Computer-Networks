using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IPv6Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float timeRemaining = 30f; // 30 секунд на игру
    private bool isGameActive = true;
    public IPv6 ipv6Game; // —сылка на основной скрипт

    void Update()
    {
        if (isGameActive)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = $"Time Left: {Mathf.Ceil(timeRemaining)}";

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
        foreach (Button button in ipv6Game.categoryButtons)
        {
            button.interactable = false;
        }
        ipv6Game.feedbackText.text = $"Your score is: {ipv6Game.score}";
    }
}
