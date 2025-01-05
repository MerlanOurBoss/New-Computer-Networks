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
            Debug.LogError("Ошибка: Нет данных о вошедшем пользователе.");
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
            Debug.Log("Данные пользователя загружены.");
        }
        else
        {
            Debug.LogError("Файл с данными пользователя не найден.");
        }
    }

    private void DisplayUserWindow()
    {
        if (userData.userType == UserType.Teacher)
        {
            teacherWindow.SetActive(true);
            welcomeText.text = "Учитель " + userData.name;
        }
        else
        {
            teacherWindow.SetActive(false);
            welcomeText.text = "Студент " + userData.name;
        }
    }
}
