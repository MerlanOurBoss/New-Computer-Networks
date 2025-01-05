using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; // ������������ ��� ������ ������ �� �����
using Newtonsoft.Json; // ���������� ���������� Newtonsoft.Json

public class GoogleSheetsReader : MonoBehaviour
{
    private string apiKey = "-"; // �������� �� ��� API ����
    private string spreadsheetId = "-"; // �������� �� ID ����� �������
    private string range = "����1!A1:B5"; // ��������, �� �������� ��������� ������ (������� A � B)

    public TextMeshProUGUI studentNamesText; // ��������� TextMeshPro ������� �� Unity ��� ������ ������

    void Start()
    {
        if (studentNamesText == null)
        {
            Debug.LogError("TextMeshProUGUI �� ��������. ���������� ��������� ������ � ���������.");
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
            Debug.Log("����� �� Google Sheets: " + jsonResult); // ������� ����� ��� �������

            // ������������� JSON-����� � ������� Newtonsoft.Json
            GoogleSheetResponse response = JsonConvert.DeserializeObject<GoogleSheetResponse>(jsonResult);

            // ��������� ������� ��������
            if (response.values != null && response.values.Length > 0)
            {
                Debug.Log("������ �������, ���������� �����: " + response.values.Length);
                string studentNamesAndGrades = "";
                foreach (var row in response.values)
                {
                    // ���������, ��� � ������ ���� ��������
                    if (row.Length > 1)
                    {
                        studentNamesAndGrades += row[0] + " - " + row[1] + "\n"; // ��������� ��� �������� � ������ � ����� ������
                    }
                }

                // ���������, ��� studentNamesAndGrades �� ����
                if (!string.IsNullOrEmpty(studentNamesAndGrades))
                {
                    studentNamesText.text = studentNamesAndGrades; // ������������� ����� �� TextMeshPro ������
                }
                else
                {
                    Debug.LogError("������: �� ������� �������� ����� ��������� � �� ������.");
                }
            }
            else
            {
                Debug.LogError("������: ������ ������ �� Google Sheets.");
            }
        }
        else
        {
            Debug.LogError("������ �������: " + request.error);
        }
    }
}

// ����� ��� �������������� ������ �� Google Sheets
public class GoogleSheetResponse
{
    public string range;
    public string majorDimension;
    public string[][] values; // ���������� ������ ����� ��� ��������
}
