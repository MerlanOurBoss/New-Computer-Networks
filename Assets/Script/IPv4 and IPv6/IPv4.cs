using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IPv4 : MonoBehaviour
{
    public TextMeshProUGUI ipAddressText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI scoreText;
    public Button[] classButtons;  //  нопки дл€ классов A, B, C, D, E

    private string currentIPAddress;
    private char correctClass;
    public int score = 0;

    void Start()
    {
        GenerateNewIPAddress();
    }

    void GenerateNewIPAddress()
    {
        int[] octets = new int[4];
        for (int i = 0; i < 4; i++)
        {
            octets[i] = Random.Range(0, 256);  // √енерируем случайный октет от 0 до 255
        }

        currentIPAddress = $"{octets[0]}.{octets[1]}.{octets[2]}.{octets[3]}";
        ipAddressText.text = $"{currentIPAddress}";
        correctClass = DetermineClass(octets[0]);
        Debug.Log(correctClass);
    }

    char DetermineClass(int firstOctet)
    {
        if (firstOctet >= 1 && firstOctet <= 126)
            return 'A';
        else if (firstOctet >= 128 && firstOctet <= 191)
            return 'B';
        else if (firstOctet >= 192 && firstOctet <= 223)
            return 'C';
        else if (firstOctet >= 224 && firstOctet <= 239)
            return 'D';
        else
            return 'E';
    }

    public void OnClassButtonClicked(string classLabel)
    {
        if (classLabel[0] == correctClass)
        {
            feedbackText.text = "Correct!";
            score += 10;
        }
        else
        {
            feedbackText.text = $"Wrong! Correct class was {correctClass}.";
            score -= 5;
            Debug.Log("wrong");
        }

        UpdateScoreUI();
        GenerateNewIPAddress();
    }

    void UpdateScoreUI()
    {
        scoreText.text = $"Score: {score}";
    }
}
