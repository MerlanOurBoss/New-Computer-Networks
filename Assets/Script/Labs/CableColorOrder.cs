using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CableColorOrder : MonoBehaviour
{
    public TMP_Dropdown standardDropdown; // Dropdown для выбора стандарта
    public TMP_Dropdown[] colorDropdowns; // Поля ввода для установки цветов
    public TMP_Text resultText; // Текст для отображения результата проверки

    public bool isCorrect = false;

    // Цвета для T-568A
    private readonly string[] T568AColors = new string[]
    {
        "ақ-жасыл",
        "жасыл",
        "ақ-сары",
        "көк",
        "ақ-көк",
        "сары",
        "ақ-қоңыр",
        "қоңыр"
    };

    // Цвета для T-568B
    private readonly string[] T568BColors = new string[]
    {
        "ақ-сары",
        "сары",
        "ақ-жасыл",
        "көк",
        "ақ-көк",
        "жасыл",
        "ақ-қоңыр",
        "қоңыр"
    };

    private string[] currentStandardColors;

    void Start()
    {
        // Подписываемся на событие изменения выбора стандарта
        standardDropdown.onValueChanged.AddListener(SetCurrentStandard);

        // Устанавливаем текущий стандарт и обновляем UI
        SetCurrentStandard(standardDropdown.value);

        // Инициализируем dropdowns цветов
        InitializeColorDropdowns();
    }

    // Метод для установки текущего стандарта
    private void SetCurrentStandard(int standardIndex)
    {
        currentStandardColors = standardIndex == 0 ? T568AColors : T568BColors;
        resultText.text = ""; // Очищаем текст результата

        // Сбрасываем dropdowns цветов
        foreach (var dropdown in colorDropdowns)
        {
            dropdown.value = 0;
        }

        // Перемешиваем цвета в каждом dropdown
        InitializeColorDropdowns();
    }

    // Метод для инициализации dropdowns цветов
    private void InitializeColorDropdowns()
    {
        foreach (var dropdown in colorDropdowns)
        {
            List<string> randomizedColors = T568AColors.OrderBy(c => Random.value).ToList();
            dropdown.ClearOptions();
            dropdown.AddOptions(randomizedColors);
        }
    }

    // Метод для проверки правильности порядка цветов
    public void CheckColorOrder()
    {
        for (int i = 0; i < colorDropdowns.Length; i++)
        {
            if (i < currentStandardColors.Length)
            {
                if (!colorDropdowns[i].options[colorDropdowns[i].value].text.Equals(currentStandardColors[i], System.StringComparison.OrdinalIgnoreCase))
                {
                    resultText.text = "Порядок цветов неверный!";
                    resultText.color = Color.red;
                    isCorrect = false;
                    return;
                }
            }
        }

        resultText.text = "Порядок цветов правильный!";
        resultText.color = Color.green;
        isCorrect = true;
    }
}
