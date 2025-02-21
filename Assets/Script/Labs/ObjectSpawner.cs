using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject pcPrefab;       // ������ ��� PC
    public GameObject routerPrefab;   // ������ ��� Router
    public GameObject switchPrefab;   // ������ ��� Switch
    public Canvas canvas;             // ������ �� Canvas
    public ConnectionManager connectionManager; // ������ �� ConnectionManager

    private GameObject currentObject; // ������� ����������� ������
    private bool isPlacingObject = false; // ���� ���������� �������
    void Update()
    {
        // ���� � �������� ���������� �������
        if (isPlacingObject && currentObject != null)
        {
            // ��������� �������� ���������� ���� � ��������� ���������� Canvas
            Vector2 mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out mousePosition
            );

            // ������������� ������� �������
            currentObject.GetComponent<RectTransform>().anchoredPosition = mousePosition;

            // ��������� ���������� ��� ����� ����� �������
            if (Input.GetMouseButtonDown(0))
            {
                isPlacingObject = false; // ��������� ����������
                currentObject = null;
            }
        }
    }

    // ����� ��� ������ �������
    public void SpawnObject(string objectType)
    {
        if (isPlacingObject) return; // ���� ������ ��� �����������, ����������

        GameObject prefabToSpawn = null;

        // ����������, ����� ������ ���������
        switch (objectType)
        {
            case "PC":
                prefabToSpawn = pcPrefab;
                GameObject taggedObject = GameObject.FindWithTag("PCToggle");
                if (taggedObject != null)
                {
                    Debug.Log("dainsdoia");
                    Toggle toggle = taggedObject.GetComponent<Toggle>();
                    if (toggle != null)
                    {
                        if (!toggle.isOn)
                        {
                            toggle.isOn = true;
                        }
                    }
                }
                break;
            case "Router":
                prefabToSpawn = routerPrefab;
                GameObject taggedObject1 = GameObject.FindWithTag("RouterToggle");
                if (taggedObject1 != null)
                {
                    Debug.Log("dainsdoia");
                    Toggle toggle = taggedObject1.GetComponent<Toggle>();
                    if (toggle != null)
                    {
                        if (!toggle.isOn)
                        {
                            toggle.isOn = true;
                        }
                    }
                }
                break;
            case "Switch":
                prefabToSpawn = switchPrefab;
                GameObject taggedObject2 = GameObject.FindWithTag("SwitchToggle");
                if (taggedObject2 != null)
                {
                    Debug.Log("dainsdoia");
                    Toggle toggle = taggedObject2.GetComponent<Toggle>();
                    if (toggle != null)
                    {
                        if (!toggle.isOn)
                        {
                            toggle.isOn = true;
                        }
                    }
                }
                break;
        }

        if (prefabToSpawn != null)
        {
            // ������ ������ ������ Canvas
            currentObject = Instantiate(prefabToSpawn, canvas.transform);

            // �������� ������� �������
            RectTransform rectTransform = currentObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;  // ������������� (0, 0)
            rectTransform.localPosition = Vector3.zero;     // ������������� ���������� Z, ���� �����

            // ��������� �������������� � �������
            Button button = currentObject.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = false;
            }

            isPlacingObject = true; // �������� ����� ����������
        }
    }
}
