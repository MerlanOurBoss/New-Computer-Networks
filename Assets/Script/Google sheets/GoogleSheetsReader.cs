using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; // Используется для вывода текста на экран
using Newtonsoft.Json; // Подключаем библиотеку Newtonsoft.Json

public class GoogleSheetsReader : MonoBehaviour
{
    private string apiKey = "-"; // Замените на ваш API ключ
    private string spreadsheetId = "-"; // Замените на ID вашей таблицы
    private string range = "Лист1!A1:B5"; // Диапазон, из которого считываем данные (колонки A и B)

    public TextMeshProUGUI studentNamesText; // Привяжите TextMeshPro элемент из Unity для вывода текста

    void Start()
    {
        if (studentNamesText == null)
        {
            Debug.LogError("TextMeshProUGUI не привязан. Перетащите текстовый объект в инспектор.");
            return;
        }

        StartCoroutine(GetDataFromGoogleSheets());
    }

    IEnumerator GetDataFromGoogleSheets()
    {
        string url = $"https://sheets.googleapis.com/v4/spreadsheets/{spreadsheetId}/values/{range}?key={apiKey}";

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            Debug.Log("Ответ от Google Sheets: " + jsonResult); // Выводим ответ для отладки

            // Десериализуем JSON-ответ с помощью Newtonsoft.Json
            GoogleSheetResponse response = JsonConvert.DeserializeObject<GoogleSheetResponse>(jsonResult);

            // Проверяем наличие значений
            if (response.values != null && response.values.Length > 0)
            {
                Debug.Log("Данные найдены, количество строк: " + response.values.Length);
                string studentNamesAndGrades = "";
                foreach (var row in response.values)
                {
                    // Проверяем, что в строке есть значения
                    if (row.Length > 1)
                    {
                        studentNamesAndGrades += row[0] + " - " + row[1] + "\n"; // Добавляем имя студента и оценку с новой строки
                    }
                }

                // Проверяем, что studentNamesAndGrades не пуст
                if (!string.IsNullOrEmpty(studentNamesAndGrades))
                {
                    studentNamesText.text = studentNamesAndGrades; // Устанавливаем текст на TextMeshPro объект
                }
                else
                {
                    Debug.LogError("Ошибка: Не удалось получить имена студентов и их оценки.");
                }
            }
            else
            {
                Debug.LogError("Ошибка: Пустые данные из Google Sheets.");
            }
        }
        else
        {
            Debug.LogError("Ошибка запроса: " + request.error);
        }
    }
}

// Класс для десериализации ответа от Google Sheets
public class GoogleSheetResponse
{
    public string range;
    public string majorDimension;
    public string[][] values; // Используем массив строк для значений
}
