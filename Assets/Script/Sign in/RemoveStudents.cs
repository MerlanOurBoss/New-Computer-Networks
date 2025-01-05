using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.Networking;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class RemoveStudents : MonoBehaviour
{
    public TextMeshProUGUI studentNameInput; // Поле ввода имени студента

    private string spreadsheetId = "1xfhuIBhpGMJqlTdlStraXI2GHFceZFaSPi9ACgPgL34"; // ID таблицы
    private SheetsService sheetsService;

    private void Start()
    {
        Authenticate();
    }

    // Метод для аутентификации с использованием сервисного аккаунта
    private void Authenticate()
    {
        // Загрузка учетных данных из JSON-файла
        string jsonPath = Path.Combine(Application.streamingAssetsPath, "handy-buffer-439916-t0-c14443848c99.json");
        GoogleCredential credential;

        using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(SheetsService.Scope.Spreadsheets);
        }

        // Создание сервиса Google Sheets
        sheetsService = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Computer Networks Project",
        });
    }

    // Метод для удаления студента
    public void RemoveStudent()
    {
        string studentName = studentNameInput.text.ToString(); // Получаем имя студента из поля ввода
        if (string.IsNullOrEmpty(studentName))
        {
            return;
        }

        StartCoroutine(RemoveStudentFromGoogleSheets(studentName));
    }

    // Метод для отправки запроса на удаление студента
    private IEnumerator RemoveStudentFromGoogleSheets(string studentName)
    {
        string range = "TeacherAndStudents!A2:C100"; // Expand range to cover up to 100 rows
        SpreadsheetsResource.ValuesResource.GetRequest getRequest = sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
        ValueRange response = getRequest.Execute();

        if (response.Values != null && response.Values.Count > 0)
        {
            int rowIndexToRemove = -1;

            // Iterate over all rows, including empty rows
            for (int i = 0; i < 100; i++) // Adjusted to check up to 100 rows in range
            {
                // Ensure row data exists within response.Values, or set as empty
                var currentRow = i < response.Values.Count ? response.Values[i] : new List<object> { "", "", "" };

                // Check if the first cell matches the student name
                if (currentRow.Count > 0 && currentRow[0].ToString().Equals(studentName, System.StringComparison.OrdinalIgnoreCase))
                {
                    rowIndexToRemove = i + 2; // Calculate Google Sheets row index
                    break;
                }
            }

            if (rowIndexToRemove != -1)
            {
                Debug.Log("Removing student at row: " + rowIndexToRemove);

                string deleteRange = $"TeacherAndStudents!A{rowIndexToRemove}:C{rowIndexToRemove}";
                var requestBody = new ValueRange { Values = new[] { new[] { "", "", "" } } };
                var updateRequest = sheetsService.Spreadsheets.Values.Update(requestBody, spreadsheetId, deleteRange);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

                // Execute update request
                var updateResponse = updateRequest.Execute();
                Destroy(gameObject); // Optional: Destroy the object after update
            }
        }

        yield return null;
    }
}
