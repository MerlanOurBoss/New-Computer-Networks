using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.IO;

public class AddStudent : MonoBehaviour
{
    public TMP_InputField nameField;
    public TMP_InputField passwordField;
    public TextMeshProUGUI resultText;

    private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    private SheetsService sheetsService;
    private string spreadsheetId = "1xfhuIBhpGMJqlTdlStraXI2GHFceZFaSPi9ACgPgL34"; // ID таблицы
    private string sheetName = "TeacherAndStudents"; // Название листа в таблице

    private void Start()
    {
        InitializeGoogleSheetsService();
    }

    private void InitializeGoogleSheetsService()
    {
        string jsonPath = Path.Combine(Application.streamingAssetsPath, "handy-buffer-439916-t0-c14443848c99.json");

        if (File.Exists(jsonPath))
        {
            GoogleCredential credential;
            using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            }

            sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Computer Networks Project"
            });

            resultText.text = "Сервис Google Sheets успешно инициализирован!";
        }
        else
        {
            resultText.text = "Ошибка: JSON-файл ключа сервисного аккаунта не найден.";
        }
    }

    public void AddStudents()
    {
        string studentName = nameField.text.Trim();
        string studentPassword = passwordField.text.Trim();

        if (!string.IsNullOrEmpty(studentName) && !string.IsNullOrEmpty(studentPassword))
        {
            StartCoroutine(AddStudentToGoogleSheets(studentName, studentPassword));
        }
        else
        {
            resultText.text = "Пожалуйста, заполните оба поля.";
        }
    }

    private IEnumerator AddStudentToGoogleSheets(string name, string password)
    {
        var values = new List<IList<object>> { new List<object> { name, password, "Student" } };

        var valueRange = new ValueRange
        {
            MajorDimension = "ROWS",
            Values = values
        };

        var request = sheetsService.Spreadsheets.Values.Append(valueRange, spreadsheetId, $"{sheetName}!A4:C10");
        request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

        var appendRequestTask = request.ExecuteAsync();

        yield return new WaitUntil(() => appendRequestTask.IsCompleted);

        if (appendRequestTask.Exception == null)
        {
            resultText.text = $"Студент {name} успешно добавлен.";
            nameField.text = "";
            passwordField.text = "";
        }
        else
        {
            resultText.text = "Ошибка: Не удалось добавить студента.";
            Debug.LogError("Ошибка запроса: " + appendRequestTask.Exception.Message);
        }
    }
}
