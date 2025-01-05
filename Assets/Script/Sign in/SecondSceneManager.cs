using System.IO;
using UnityEngine;
using TMPro;

public class SecondSceneManager : MonoBehaviour
{
    public GameObject teacherWindow;
    public TextMeshProUGUI welcomeText;

    private UserData userData;

    private void Start()
    {
        LoadUserData();
        if (userData != null)
        {
            DisplayUserWindow();
        }
        else
        {
            Debug.LogError("������: ��� ������ � �������� ������������.");
        }
    }

    private void LoadUserData()
    {
        string path = Path.Combine(Application.persistentDataPath, "UserData.json");

        if (File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            userData = ScriptableObject.CreateInstance<UserData>();
            JsonUtility.FromJsonOverwrite(jsonData, userData);
            Debug.Log("������ ������������ ���������.");
        }
        else
        {
            Debug.LogError("���� � ������� ������������ �� ������.");
        }
    }

    private void DisplayUserWindow()
    {
        if (userData.userType == UserType.Teacher)
        {
            teacherWindow.SetActive(true);
            welcomeText.text = "������� " + userData.name;
        }
        else
        {
            teacherWindow.SetActive(false);
            welcomeText.text = "������� " + userData.name;
        }
    }
}
