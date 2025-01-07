using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviour
{
    public Button startConnectionButton;
    public GameObject selectionUI; // UI для выбора стандарта и кабеля
    public TMP_Dropdown standardDropdown; // Dropdown для выбора стандарта
    public TMP_Dropdown cableTypeDropdown; // Dropdown для выбора кабеля
    public GameObject linePrefab; // Префаб линии с LineRenderer

    private GameObject firstObject;
    private GameObject secondObject;
    private string firstStandard;
    private string firstCableType;
    private string secondStandard;
    private string secondCableType;

    private bool isConnecting = false;
    private bool isSelectingFirstObject = true;

    void Start()
    {
        startConnectionButton.onClick.AddListener(StartConnection);
        selectionUI.SetActive(false);
    }
    void Update()
    {
        if (isConnecting && Input.GetMouseButtonDown(0)) // ЛКМ для выбора объекта
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                SelectObject(hit.collider.gameObject);
            }
        }
    }
    void StartConnection()
    {
        isConnecting = true;
        firstObject = null;
        secondObject = null;
        Debug.Log("Начат процесс соединения объектов.");
    }

    public void SelectObject(GameObject selectedObject)
    {
        if (!isConnecting) return;

        if (firstObject == null)
        {
            firstObject = selectedObject;
            isSelectingFirstObject = true;
            ShowSelectionUI();
        }
        else if (secondObject == null && selectedObject != firstObject)
        {
            secondObject = selectedObject;
            isSelectingFirstObject = false;
            ShowSelectionUI();
        }
    }

    void ShowSelectionUI()
    {
        selectionUI.SetActive(true);
        // Настройте UI в зависимости от объекта (например, добавьте текст "Выбор для первого объекта" или "Выбор для второго объекта")
    }

    public void OnConfirmSelection()
    {
        string selectedStandard = standardDropdown.options[standardDropdown.value].text;
        string selectedCableType = cableTypeDropdown.options[cableTypeDropdown.value].text;

        SetConnectionOptions(selectedStandard, selectedCableType, isSelectingFirstObject);
    }

    public void SetConnectionOptions(string standard, string cableType, bool isFirstObject)
    {
        if (isFirstObject)
        {
            firstStandard = standard;
            firstCableType = cableType;
        }
        else
        {
            secondStandard = standard;
            secondCableType = cableType;
        }

        selectionUI.SetActive(false);

        if (!isFirstObject) // Если выбор для второго объекта завершён
        {
            ValidateAndConnect();
        }
    }

    void ValidateAndConnect()
    {
        // Пример проверки совместимости
        if (firstStandard == secondStandard && firstCableType == secondCableType)
        {
            CreateConnectionLine();
            Debug.Log("Соединение успешно создано.");
        }
        else
        {
            Debug.Log("Ошибка: Неверные параметры подключения.");
        }

        ResetConnection();
    }

    void CreateConnectionLine()
    {
        GameObject lineObject = Instantiate(linePrefab);
        LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, firstObject.transform.position);
        lineRenderer.SetPosition(1, secondObject.transform.position);
    }

    void ResetConnection()
    {
        isConnecting = false;
        firstObject = null;
        secondObject = null;
        firstStandard = null;
        firstCableType = null;
        secondStandard = null;
        secondCableType = null;
    }
}
