using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Ping = System.Net.NetworkInformation.Ping;

public class PingManager : MonoBehaviour
{
    public TMP_InputField consoleInputField; // Поле ввода "консоли"
    public TMP_Text consoleOutput;          // Текст, который имитирует вывод консоли
    public Button executeButton;            // Кнопка для выполнения команды

    public ConnectionManager connectionManager; // Набор доступных IP-адресов

    private void Start()
    {
        if (executeButton != null)
        {
            executeButton.onClick.AddListener(ExecuteCommand);
        }

        LogToConsole("Добро пожаловать в консоль PingManager.");
        LogToConsole("Введите команду: ping <IP-адрес>");
    }

    private void ExecuteCommand()
    {
        string command = consoleInputField.text;

        if (string.IsNullOrWhiteSpace(command))
        {
            LogToConsole("Ошибка: команда не введена.");
            return;
        }

        // Парсим команду
        string[] parts = command.Split(' ');
        if (parts.Length != 2 || parts[0].ToLower() != "ping")
        {
            LogToConsole("Ошибка: неверная команда. Используйте 'ping <IP-адрес>'.");
            return;
        }

        string ipAddress = parts[1];

        if (!connectionManager.assignedIps.Contains(ipAddress))
        {
            LogToConsole($"Ошибка: IP-адрес {ipAddress} не найден в доступных.");
            return;
        }

        LogToConsole($"Проверка подключения к {ipAddress}...");

        // Запуск пинга
        StartCoroutine(PingIPAddress(ipAddress));
    }

    private IEnumerator PingIPAddress(string ipAddress)
    {
        // Проверяем, существует ли объект с данным IP
        if (connectionManager.ipToComputerMap.TryGetValue(ipAddress, out GameObject computer))
        {
            // Имитация времени ответа (например, расстояние между "компьютерами")
            float distance = Vector3.Distance(this.transform.position, computer.transform.position);
            float responseTime = distance * 10; // Время ответа пропорционально расстоянию

            yield return new WaitForSeconds(responseTime / 1000f); // Имитация задержки

            LogToConsole($"Ответ от {ipAddress}: время = {responseTime:F1} мс");
        }
        else
        {
            LogToConsole($"Не удалось найти объект с IP-адресом {ipAddress}.");
        }
    }

    private void LogToConsole(string message)
    {
        if (consoleOutput != null)
        {
            consoleOutput.text += message + "\n";
        }
    }
}
