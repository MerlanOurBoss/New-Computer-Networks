using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.IO;

[System.Serializable]
public class Teacher
{
    public string name;
    public string password;
    public UserType userType;
}

public enum UserType
{
    Teacher,
    Student
}

[System.Serializable]
public class GoogleSheetResponses
{
    public string[][] values; // ������ �� Google Sheets
}

public class SignInSystem : MonoBehaviour
{
    public TMP_InputField loginField;
    public TMP_InputField passwordField;
    public TextMeshProUGUI resultText;
    public Button showPasswordButton;
    private bool isPasswordVisible = false;

    public Sprite[] sprites;

    private UserData userData; // ������ �� UserData ScriptableObject

    private List<Teacher> teachersData = new List<Teacher>();

    private string apiKey = "-"; // ��� API ����
    private string spreadsheetId = "-"; // ID �������
    private string range = "TeacherAndStudents!A2:C100"; // �������� ������

    private void Start()
    {
        passwordField.contentType = TMP_InputField.ContentType.Password;
        passwordField.ForceLabelUpdate();

        showPasswordButton.onClick.AddListener(TogglePasswordVisibility);
        showPasswordButton.image.sprite = sprites[0];

        StartCoroutine(LoadTeachersData());
    }

    IEnumerator LoadTeachersData()
    {
        string url = $"https://sheets.googleapis.com/v4/spreadsheets/{spreadsheetId}/values/{range}?key={apiKey}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            Debug.Log("����� �� Google Sheets: " + jsonResult);

            GoogleSheetResponses response = JsonConvert.DeserializeObject<GoogleSheetResponses>(jsonResult);

            if (response.values != null && response.values.Length > 0)
            {
                foreach (var row in response.values)
                {
                    if (row.Length >= 3)
                    {
                        UserType userType = row[2] == "Teacher" ? UserType.Teacher : UserType.Student;
                        teachersData.Add(new Teacher { name = row[0], password = row[1], userType = userType });
                    }
                }
                resultText.text = "������ ���������.";
            }
            else
            {
                resultText.text = "������: ������ ������ �� Google Sheets.";
            }
        }
        else
        {
            resultText.text = "������: �� ������� ��������� ������ � Google Sheets.";
            Debug.LogError("������ �������: " + request.error);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Login();
        }
    }

    // ����� ��� ����� � �������
    public void Login()
    {
        string inputLogin = loginField.text;
        string inputPassword = passwordField.text;

        foreach (Teacher teacher in teachersData)
        {
            if (teacher.name == inputLogin && teacher.password == inputPassword)
            {
                SaveLoggedInTeacher(teacher);
                resultText.text = "Login successful!";
                SceneManager.LoadScene(1);
                return;
            }
        }

        resultText.text = "Error: Invalid login or password.";
    }

    // ���������� ������ ��������� ������������
    void SaveLoggedInTeacher(Teacher teacher)
    {
        userData = ScriptableObject.CreateInstance<UserData>();
        userData.name = teacher.name;
        userData.password = teacher.password;
        userData.userType = teacher.userType;

        // ����������� ������ � JSON
        string jsonData = JsonUtility.ToJson(userData);

        // ���� ��� ���������� ����� � ������� ������
        string path = Path.Combine(Application.persistentDataPath, "UserData.json");

        File.WriteAllText(path, jsonData);
        Debug.Log("������ ������������ ��������� � JSON �� ����: " + path);
    }

    void TogglePasswordVisibility()
    {
        isPasswordVisible = !isPasswordVisible;

        if (isPasswordVisible)
        {
            passwordField.contentType = TMP_InputField.ContentType.Standard;
            showPasswordButton.image.sprite = sprites[1];
        }
        else
        {
            passwordField.contentType = TMP_InputField.ContentType.Password;
            showPasswordButton.image.sprite = sprites[0];
        }

        passwordField.ForceLabelUpdate();
    }
}
