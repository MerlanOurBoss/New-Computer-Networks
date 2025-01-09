using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PortManager : MonoBehaviour
{/// <summary>
/// Удалить это
/// </summary>
    public GameObject portSelectionPrefab; // Префаб для выбора портов
    public Transform portSelectionParent; // Родительский объект для префабов выбора портов
    public int numberOfPorts = 6; // Количество портов по умолчанию
    public TMP_Text warningText; // Текст для предупреждений

    private Dictionary<GameObject, GameObject> deviceToPortSelector = new Dictionary<GameObject, GameObject>();

    // Метод для создания устройства с портами
    public void CreateDevice(GameObject devicePrefab, Vector3 position)
    {
        // Создание устройства
        GameObject device = Instantiate(devicePrefab, position, Quaternion.identity);

        // Создание объекта для выбора портов
        GameObject portSelector = Instantiate(portSelectionPrefab, portSelectionParent);
        portSelector.SetActive(false); // Отключить до активации
        deviceToPortSelector.Add(device, portSelector);

        // Настройка портов
        PortSelector selector = portSelector.GetComponent<PortSelector>();
        selector.InitializePorts(numberOfPorts);
    }

    // Включение выбора портов для устройства
    public void ActivatePortSelection(GameObject device)
    {
        if (!deviceToPortSelector.ContainsKey(device)) return;

        GameObject portSelector = deviceToPortSelector[device];
        portSelector.SetActive(true);
    }

    // Проверка, есть ли свободные порты
    public bool HasAvailablePorts(GameObject device)
    {
        if (!deviceToPortSelector.ContainsKey(device)) return false;

        GameObject portSelector = deviceToPortSelector[device];
        PortSelector selector = portSelector.GetComponent<PortSelector>();
        return selector.HasFreePorts();
    }

    // Предупреждение о том, что порты закончились
    public void ShowWarning(string message)
    {
        warningText.text = message;
        Invoke(nameof(ClearWarning), 3f); // Очистить через 3 секунды
    }

    private void ClearWarning()
    {
        warningText.text = "";
    }
}
