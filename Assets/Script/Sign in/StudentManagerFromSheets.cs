using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;

public class StudentManagerFromSheets : MonoBehaviour
{
    public GameObject studentPrefab;
    public Transform spawnParent;
    private string apiKey = "-";
    private string spreadsheetId = "-";
    private string range = "TeacherAndStudents!A2:C100";
    private List<string> studentNames;
    private float updateInterval = 5f; // Интервал обновления в секундах
    private float lastUpdateTime;

    private void Start()
    {
        lastUpdateTime = -updateInterval; // Сразу загружаем данные при запуске
        StartCoroutine(LoadStudentNamesFromGoogleSheets());
    }

    public void Update()
    {
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            lastUpdateTime = Time.time;
            StartCoroutine(LoadStudentNamesFromGoogleSheets());
        }
    }

    IEnumerator LoadStudentNamesFromGoogleSheets()
    {
        string url = $"https://sheets.googleapis.com/v4/spreadsheets/{spreadsheetId}/values/{range}?key={apiKey}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            GoogleSheetResponse response = JsonConvert.DeserializeObject<GoogleSheetResponse>(jsonResult);

            if (response.values != null && response.values.Length > 0)
            {
                studentNames = new List<string>();
                foreach (var row in response.values)
                {
                    if (row.Length >= 3 && row[2].ToLower() == "student")
                    {
                        studentNames.Add(row[0]);
                    }
                }
                SpawnStudents(studentNames);
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

    private void SpawnStudents(List<string> studentNames)
    {
        foreach (Transform child in spawnParent)
        {
            Destroy(child.gameObject);
        }

        foreach (string name in studentNames)
        {
            GameObject studentInstance = Instantiate(studentPrefab, spawnParent);
            studentInstance.name = $"Student_{name}";

            TextMeshProUGUI nameText = studentInstance.transform.Find("StudentName").GetComponent<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = name;
            }
        }
    }
}
