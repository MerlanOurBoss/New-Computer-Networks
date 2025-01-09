using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PortManager : MonoBehaviour
{/// <summary>
/// ������� ���
/// </summary>
    public GameObject portSelectionPrefab; // ������ ��� ������ ������
    public Transform portSelectionParent; // ������������ ������ ��� �������� ������ ������
    public int numberOfPorts = 6; // ���������� ������ �� ���������
    public TMP_Text warningText; // ����� ��� ��������������

    private Dictionary<GameObject, GameObject> deviceToPortSelector = new Dictionary<GameObject, GameObject>();

    // ����� ��� �������� ���������� � �������
    public void CreateDevice(GameObject devicePrefab, Vector3 position)
    {
        // �������� ����������
        GameObject device = Instantiate(devicePrefab, position, Quaternion.identity);

        // �������� ������� ��� ������ ������
        GameObject portSelector = Instantiate(portSelectionPrefab, portSelectionParent);
        portSelector.SetActive(false); // ��������� �� ���������
        deviceToPortSelector.Add(device, portSelector);

        // ��������� ������
        PortSelector selector = portSelector.GetComponent<PortSelector>();
        selector.InitializePorts(numberOfPorts);
    }

    // ��������� ������ ������ ��� ����������
    public void ActivatePortSelection(GameObject device)
    {
        if (!deviceToPortSelector.ContainsKey(device)) return;

        GameObject portSelector = deviceToPortSelector[device];
        portSelector.SetActive(true);
    }

    // ��������, ���� �� ��������� �����
    public bool HasAvailablePorts(GameObject device)
    {
        if (!deviceToPortSelector.ContainsKey(device)) return false;

        GameObject portSelector = deviceToPortSelector[device];
        PortSelector selector = portSelector.GetComponent<PortSelector>();
        return selector.HasFreePorts();
    }

    // �������������� � ���, ��� ����� �����������
    public void ShowWarning(string message)
    {
        warningText.text = message;
        Invoke(nameof(ClearWarning), 3f); // �������� ����� 3 �������
    }

    private void ClearWarning()
    {
        warningText.text = "";
    }
}
