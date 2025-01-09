using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviour
{
    public Button startConnectionButton;
    public GameObject selectionUI;
    public TMP_Dropdown standardDropdown;
    public TMP_Dropdown cableTypeDropdown;
    public GameObject linePrefab;
    public TMP_Text tipText;

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
        UpdateTip("Қосылу үшін «Кабель» түймесін басыңыз.");
    }

    void Update()
    {
        if (isConnecting && Input.GetMouseButtonDown(0))
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
        UpdateTip("Бірінші нысанды таңдаңыз.");
        Debug.Log("Начат процесс соединения объектов.");
    }

    public void SelectObject(GameObject selectedObject)
    {
        if (!isConnecting) return;

        if (firstObject == null)
        {
            firstObject = selectedObject;
            isSelectingFirstObject = true;
            UpdateTip("Бірінші нысан үшін опцияларды таңдаңыз.");
            ShowSelectionUI();
        }
        else if (secondObject == null && selectedObject != firstObject)
        {
            secondObject = selectedObject;
            isSelectingFirstObject = false;
            UpdateTip("Екінші нысан үшін опцияларды таңдаңыз.");
            ShowSelectionUI();
        }
    }

    void ShowSelectionUI()
    {
        selectionUI.SetActive(true);
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

        if (!isFirstObject)
        {
            ValidateAndConnect();
        }
        else
        {
            UpdateTip("Екінші нысанды таңдаңыз.");
        }
    }

    void ValidateAndConnect()
    {
        string firstType = firstObject.tag; // Предполагаем, что тип объекта задаётся через тег
        string secondType = secondObject.tag;

        // Проверка совместимости типов устройств, стандартов и кабелей
        if (IsConnectionValid(firstType, secondType, firstStandard, secondStandard, firstCableType, secondCableType))
        {
            CreateConnectionLine();
            UpdateTip("Қосылым сәтті жасалды.");
            Debug.Log("Соединение успешно создано.");
        }
        else
        {
            UpdateTip("Қате: қосылым параметрлері жарамсыз.");
            Debug.Log("Ошибка: Неверные параметры подключения.");
        }

        ResetConnection();
    }

    bool IsConnectionValid(string firstType, string secondType, string firstStandard, string secondStandard, string firstCableType, string secondCableType)
    {
        // Проверка совместимости для PC -> PC
        if (firstType == "PC" && secondType == "PC" &&
            firstStandard == secondStandard &&
            firstCableType == "Кроссовер" && secondCableType == "Кроссовер")
        {
            return true;
        }

        // Проверка совместимости для PC -> Router
        if ((firstType == "PC" && secondType == "Router" || firstType == "Router" && secondType == "PC") &&
            firstStandard == "T-568B" && secondStandard == "T-568B" &&
            firstCableType == "Тікелей" && secondCableType == "Тікелей")
        {
            return true;
        }

        // Проверка совместимости для Router -> Switch
        if ((firstType == "Router" && secondType == "Switch" || firstType == "Switch" && secondType == "Router") &&
            firstStandard == "T-568B" && secondStandard == "T-568B" &&
            firstCableType == "Тікелей" && secondCableType == "Тікелей")
        {
            return true;
        }

        // Проверка совместимости для PC -> Switch
        if ((firstType == "PC" && secondType == "Switch" || firstType == "Switch" && secondType == "PC") &&
            firstStandard == "T-568B" && secondStandard == "T-568B" &&
            firstCableType == "Тікелей" && secondCableType == "Тікелей")
        {
            return true;
        }
        return false;
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

    void UpdateTip(string message)
    {
        tipText.text = message;
    }
}
