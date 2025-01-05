using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IPv6 : MonoBehaviour
{
    public TextMeshProUGUI ipv6AddressText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI scoreText;
    public Button[] categoryButtons; // Кнопки для категорий (Unicast, Multicast и т.д.)

    private string currentIPv6Address;
    private string correctCategory;
    public int score = 0;

    void Start()
    {
        GenerateNewIPv6Address();
    }

    void GenerateNewIPv6Address()
    {
        correctCategory = DetermineCategory(); // Сначала определяем категорию

        string[] hextets = new string[8];

        switch (correctCategory)
        {
            case "Global Unicast":
                hextets[0] = Random.Range(0x2000, 0x3FFF + 1).ToString("X");
                break;
            case "Link-Local":
                hextets[0] = Random.Range(0xFE80, 0xFFFF + 1).ToString("X");
                break;
            case "Multicast":
                hextets[0] = Random.Range(0xFF00, 0xFFEF + 1).ToString("X");
                break;
            case "Other":
                hextets[0] = Random.Range(0x0000, 0x1FFF + 1).ToString("X"); // Генерируем случайный неиспользуемый диапазон
                break;
        }

        // Заполняем остальные хекстеты случайными числами
        for (int i = 1; i < 8; i++)
        {
            hextets[i] = Random.Range(0, 0xFFFF + 1).ToString("X");
        }

        currentIPv6Address = string.Join(":", hextets);
        ipv6AddressText.text = $"IPv6 Address: {currentIPv6Address}";

    }

    string DetermineCategory()
    {
        // Задаем вероятности для каждой категории
        float globalUnicastWeight = 0.35f; // 35%
        float linkLocalWeight = 0.30f;     // 30%
        float multicastWeight = 0.25f;    // 25%
        float otherWeight = 0.10f;        // 10%

        // Генерируем случайное число от 0 до 1
        float randomValue = Random.value;

        if (randomValue < globalUnicastWeight)
            return "Global Unicast";
        else if (randomValue < globalUnicastWeight + linkLocalWeight)
            return "Link-Local";
        else if (randomValue < globalUnicastWeight + linkLocalWeight + multicastWeight)
            return "Multicast";
        else
            return "Other";
    }

    public void OnCategoryButtonClicked(string category)
    {
        if (category == correctCategory)
        {
            feedbackText.text = "Correct!";
            score += 10;
        }
        else
        {
            feedbackText.text = $"Wrong! Correct category was {correctCategory}.";
            score -= 5;
        }

        UpdateScoreUI();
        GenerateNewIPv6Address();
    }

    void UpdateScoreUI()
    {
        scoreText.text = $"Score: {score}";
    }
}
