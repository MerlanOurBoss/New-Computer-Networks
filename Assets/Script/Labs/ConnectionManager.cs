using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
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

    [Header("Switch")]
    [SerializeField] private GameObject switchTogglePrefab;

    [Header("Router")]
    [SerializeField] private GameObject routerTogglePrefab;

    [Space]
    [SerializeField] private GameObject parentForPrefab;

    private GameObject firstObject;
    private GameObject secondObject;
    private string firstStandard;
    private string firstCableType;
    private string secondStandard;
    private string secondCableType;

    private bool isConnecting = false;
    private bool isSelectingFirstObject = true;
    private Dictionary<GameObject, GameObject> objectPrefabs;


    void Start()
    {
        startConnectionButton.onClick.AddListener(StartConnection);
        selectionUI.SetActive(false);
        UpdateTip("Қосылу үшін «Кабель» түймесін басыңыз.");

        // Инициализация словаря для хранения созданных префабов
        objectPrefabs = new Dictionary<GameObject, GameObject>();
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
            SpawnPrefabForSwitchRouter(selectedObject);
            ShowSelectionUI();
        }
        else if (secondObject == null && selectedObject != firstObject)
        {
            secondObject = selectedObject;
            isSelectingFirstObject = false;
            UpdateTip("Екінші нысан үшін опцияларды таңдаңыз.");
            SpawnPrefabForSwitchRouter(selectedObject);
            ShowSelectionUI();
        }
    }

    void SpawnPrefabForSwitchRouter(GameObject obj)
    {
        if (objectPrefabs.ContainsKey(obj))
        {
            // Активируем существующий префаб
            objectPrefabs[obj].SetActive(true);
        }
        else
        {
            GameObject prefab = null;

            if (obj.CompareTag("Switch"))
            {
                prefab = Instantiate(switchTogglePrefab, parentForPrefab.transform);
            }
            else if (obj.CompareTag("Router"))
            {
                prefab = Instantiate(routerTogglePrefab, parentForPrefab.transform);
            }

            if (prefab != null)
            {
                // Сохраняем ссылку на созданный префаб
                objectPrefabs[obj] = prefab;
            }
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

        // Определяем текущий объект
        GameObject currentObject = isSelectingFirstObject ? firstObject : secondObject;

        // Проверяем, что текущий объект не равен null
        if (currentObject != null && objectPrefabs.ContainsKey(currentObject) && objectPrefabs[currentObject] != null)
        {
            // Ищем Toggle с тэгом "Port" внутри префаба
            GameObject prefab = objectPrefabs[currentObject];
            Toggle[] toggles = prefab.GetComponentsInChildren<Toggle>();

            bool isValidPortSelected = false;

            foreach (var toggle in toggles)
            {
                if (toggle.CompareTag("Port") && !toggle.isOn)
                {
                    // Свободный порт найден и выбран
                    toggle.isOn = true;
                    isValidPortSelected = true;
                    break;
                }
            }

            if (!isValidPortSelected)
            {
                UpdateTip("Қате: барлық порттар бос емес. Еркін портты таңдаңыз.");
                Debug.Log("Ошибка: нет доступных портов. Выберите свободный порт.");
                return; // Завершаем выполнение метода, оставляя selectionUI активным
            }

            // Если порт выбран, отключаем активный префаб
            objectPrefabs[currentObject].SetActive(false);
        }

        // Сохраняем параметры подключения
        SetConnectionOptions(selectedStandard, selectedCableType, isSelectingFirstObject);

        if (currentObject.CompareTag("Router") || currentObject.CompareTag("Switch"))
        {
            objectPrefabs[currentObject].SetActive(false);
        }
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
        string firstType = firstObject.tag;
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

        // Деактивируем все префабы
        foreach (var prefab in objectPrefabs.Values)
        {
            prefab.SetActive(false);
        }
    }

    void UpdateTip(string message)
    {
        tipText.text = message;
    }
}
