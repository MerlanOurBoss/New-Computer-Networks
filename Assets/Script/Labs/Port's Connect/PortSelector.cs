using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortSelector : MonoBehaviour
{/// <summary>
/// Удалить это
/// </summary>
    public GameObject portTogglePrefab; // Префаб для Toggle порта
    public Transform portToggleParent; // Родительский объект для Toggle

    private List<Toggle> portToggles = new List<Toggle>();

    // Инициализация портов
    public void InitializePorts(int numberOfPorts)
    {
        for (int i = 0; i < numberOfPorts; i++)
        {
            GameObject toggleObject = Instantiate(portTogglePrefab, portToggleParent);
            Toggle toggle = toggleObject.GetComponent<Toggle>();
            portToggles.Add(toggle);
        }
    }

    // Проверка наличия свободных портов
    public bool HasFreePorts()
    {
        foreach (Toggle toggle in portToggles)
        {
            if (!toggle.isOn) return true;
        }
        return false;
    }

    // Выбор порта
    public void SelectPort(int portIndex)
    {
        if (portIndex < 0 || portIndex >= portToggles.Count) return;

        Toggle toggle = portToggles[portIndex];
        if (!toggle.isOn)
        {
            toggle.isOn = true;
            Debug.Log($"Порт {portIndex + 1} подключён.");
        }
        else
        {
            Debug.LogWarning($"Порт {portIndex + 1} уже занят!");
        }
    }
}
