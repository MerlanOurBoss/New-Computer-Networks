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
                //// ���������� �������������� � �������
                //Button button = currentObject.GetComponent<Button>();
                //if (button != null)
                //{
                //    button.interactable = true;
                //    Debug.Log("asdopanidpoianw");
                //    // ��������� ����� SelectObject ��� listener
                //    button.onClick.AddListener(() =>
                //    {
                //        connectionManager.SelectObject(currentObject);
                //    });
                //}

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
                break;
            case "Router":
                prefabToSpawn = routerPrefab;
                break;
            case "Switch":
                prefabToSpawn = switchPrefab;
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
