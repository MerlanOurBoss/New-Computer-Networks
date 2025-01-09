using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortSelector : MonoBehaviour
{/// <summary>
/// ������� ���
/// </summary>
    public GameObject portTogglePrefab; // ������ ��� Toggle �����
    public Transform portToggleParent; // ������������ ������ ��� Toggle

    private List<Toggle> portToggles = new List<Toggle>();

    // ������������� ������
    public void InitializePorts(int numberOfPorts)
    {
        for (int i = 0; i < numberOfPorts; i++)
        {
            GameObject toggleObject = Instantiate(portTogglePrefab, portToggleParent);
            Toggle toggle = toggleObject.GetComponent<Toggle>();
            portToggles.Add(toggle);
        }
    }

    // �������� ������� ��������� ������
    public bool HasFreePorts()
    {
        foreach (Toggle toggle in portToggles)
        {
            if (!toggle.isOn) return true;
        }
        return false;
    }

    // ����� �����
    public void SelectPort(int portIndex)
    {
        if (portIndex < 0 || portIndex >= portToggles.Count) return;

        Toggle toggle = portToggles[portIndex];
        if (!toggle.isOn)
        {
            toggle.isOn = true;
            Debug.Log($"���� {portIndex + 1} ���������.");
        }
        else
        {
            Debug.LogWarning($"���� {portIndex + 1} ��� �����!");
        }
    }
}
