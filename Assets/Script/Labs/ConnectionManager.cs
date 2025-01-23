using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class ConnectionManager : MonoBehaviour
{
    public Button startConnectionButton;
    public GameObject selectionUI;
    public TMP_Dropdown standardDropdown;
    public TMP_Dropdown cableTypeDropdown;
    public GameObject linePrefab;
    public TMP_Text tipText;

    [Header("IP Address Assignment")]
    public GameObject ipAssignmentUI; // UI для ввода IP
    public TMP_InputField ipInputField; // Поле для ввода IP
    private GameObject selectedForIpAssignment; // Объект, которому назначается IP
    public HashSet<string> assignedIps = new HashSet<string>();
    public Dictionary<string, GameObject> ipToComputerMap = new Dictionary<string, GameObject>();

    [Header("VLAN Assignment")]
    public GameObject vlanAssignmentUI; // UI for VLAN assignment
    public TMP_InputField vlanInputField; // Input field for VLAN ID
    private GameObject selectedForVlanAssignment; // Object to assign VLAN to
    private HashSet<string> assignedVlans = new HashSet<string>();

    [Header("Switch")]
    [SerializeField] private GameObject switchTogglePrefab;

    [Header("Router")]
    [SerializeField] private GameObject routerTogglePrefab;

    [Space]
    [SerializeField] private GameObject parentForPrefab;

    [Header("Color of Standart")]
    [SerializeField] private GameObject colorStandartUI;
    private bool isForFirstColorStandart = false;

    private GameObject firstObject;
    private GameObject secondObject;
    private string firstStandard;
    private string firstCableType;
    private string secondStandard;
    private string secondCableType;

    private bool isConnecting = false;
    private bool isIPadresswrited = false;
    private bool isVLANswrited = false;
    private bool isSelectingFirstObject = true;
    private Dictionary<GameObject, GameObject> objectPrefabs;

    private int countOfConnect = 0;
    // Словарь для хранения подключений: ключ — коммутатор, значение — список подключенных устройств
    private Dictionary<GameObject, List<GameObject>> switchConnections = new Dictionary<GameObject, List<GameObject>>();

    void Start()
    {
        startConnectionButton.onClick.AddListener(StartConnection);
        selectionUI.SetActive(false);
        ipAssignmentUI.SetActive(false);
        vlanAssignmentUI.SetActive(false);
        colorStandartUI.SetActive(false);
        UpdateTip("Қосылу үшін «Кабель» түймесін басыңыз.");

        // Инициализация словаря для хранения созданных префабов
        objectPrefabs = new Dictionary<GameObject, GameObject>();
    }
    void Update()
    {
        if (isConnecting && UnityEngine.Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
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
        countOfConnect++;
        if (firstObject == null)
        {
            firstObject = selectedObject;
            isSelectingFirstObject = true;
            UpdateTip("Бірінші нысан үшін опцияларды таңдаңыз.");
            SpawnPrefabForSwitchRouter(selectedObject);
            ShowSelectionUI();
            OpenIpAssignmentUI(selectedObject);
            OpenVlanAssignmentUI(selectedObject);
            if (countOfConnect <= 2)
            {
                colorStandartUI.SetActive(true);
            }
        }
        else if (secondObject == null && selectedObject != firstObject)
        {
            secondObject = selectedObject;
            isSelectingFirstObject = false;
            UpdateTip("Екінші нысан үшін опцияларды таңдаңыз.");
            SpawnPrefabForSwitchRouter(selectedObject);
            ShowSelectionUI();
            OpenIpAssignmentUI(selectedObject);
            OpenVlanAssignmentUI(selectedObject);
            if (countOfConnect <= 2)
            {
                colorStandartUI.SetActive(true);
            }
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

        GameObject currentObject = isSelectingFirstObject ? firstObject : secondObject;
        CableColorOrder cableColorOrder = gameObject.GetComponent<CableColorOrder>();

        if (currentObject != null && objectPrefabs.ContainsKey(currentObject) && objectPrefabs[currentObject] != null)
        {
            GameObject prefab = objectPrefabs[currentObject];
            Toggle[] toggles = prefab.GetComponentsInChildren<Toggle>();

            Toggle selectedToggle = null;

            foreach (var toggle in toggles)
            {
                if (toggle.CompareTag("Port") && toggle.isOn)
                {
                    // Проверяем, не занят ли порт
                    if (toggle.interactable)
                    {
                        selectedToggle = toggle;
                        break;
                    }
                }
            }

            if (selectedToggle == null)
            {
                // Если порт не выбран, выводим сообщение об ошибке и выходим из метода
                UpdateTip("Қате: Порт таңдалмады. Қосылу үшін портты таңдаңыз.");
                Debug.Log("Ошибка: Не выбран порт. Пожалуйста, выберите порт для подключения.");
                return;
            }

            // Сохраняем выбранный порт в объекте для текущего подключения
            if (isSelectingFirstObject)
            {
                firstObject.GetComponent<ConnectionData>().SelectedPort = selectedToggle;
            }
            else
            {
                secondObject.GetComponent<ConnectionData>().SelectedPort = selectedToggle;
            }

            objectPrefabs[currentObject].SetActive(false);
        }
        ConnectionData connectionData = currentObject.GetComponent<ConnectionData>();
        if (connectionData.IPAddressText.text == null)
        {
            AssignIpAddress();
        }
        if (currentObject.CompareTag("Switch"))
        {
            if (connectionData.VLANText.text == null)
            {
                Debug.Log("asdpiahnpdiunwpieoufnopwierufgbeoirubgfesopirjgfne");
                AssignVlan();
            }
        }

        if (countOfConnect <= 2)
        {
            cableColorOrder.CheckColorOrder();
        }

        if (!isIPadresswrited && !isVLANswrited)
        {
            if (countOfConnect <= 2)
            {
                if (cableColorOrder.isCorrect)
                {
                    SetConnectionOptions(selectedStandard, selectedCableType, isSelectingFirstObject);
                    ipAssignmentUI.SetActive(false);
                    vlanAssignmentUI.SetActive(false);
                    colorStandartUI.SetActive(false);
                }
            }
            else
            {
                SetConnectionOptions(selectedStandard, selectedCableType, isSelectingFirstObject);
                ipAssignmentUI.SetActive(false);
                vlanAssignmentUI.SetActive(false);
            }

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

        if (IsConnectionValid(firstType, secondType, firstStandard, secondStandard, firstCableType, secondCableType))
        {
            // Если соединение валидно, создаем линию и блокируем порты
            CreateConnectionLine();
            BlockSelectedPort(firstObject);
            BlockSelectedPort(secondObject);

            // Добавляем подключение к словарю
            AddConnection(firstObject, secondObject);
            AddConnection(secondObject, firstObject);

            UpdateTip("Қосылым сәтті жасалды.");
            Debug.Log("Соединение успешно создано.");
        }
        else
        {
            ReleaseSelectedPort(firstObject);
            ReleaseSelectedPort(secondObject);
            UpdateTip("Қате: қосылым параметрлері жарамсыз.");
            Debug.Log("Ошибка: Неверные параметры подключения.");
        }

        ResetConnection();
    }
    void BlockSelectedPort(GameObject obj)
    {
        if (obj == null) return;

        ConnectionData connectionData = obj.GetComponent<ConnectionData>();
        if (connectionData != null && connectionData.SelectedPort != null)
        {
            connectionData.SelectedPort.interactable = false;
        }
    }
    void ReleaseSelectedPort(GameObject obj)
    {
        if (obj == null) return;

        ConnectionData connectionData = obj.GetComponent<ConnectionData>();
        if (connectionData != null && connectionData.SelectedPort != null)
        {
            connectionData.SelectedPort.isOn = false;
            connectionData.SelectedPort = null; // Сбрасываем ссылку на порт
        }
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
    public void OpenVlanAssignmentUI(GameObject obj)
    {
        // Ensure VLAN can only be assigned to switches
        if (obj.CompareTag("Switch"))
        {
            selectedForVlanAssignment = obj;

            // Check if VLAN is already assigned
            ConnectionData connectionData = obj.GetComponent<ConnectionData>();
            if (connectionData != null && !string.IsNullOrEmpty(connectionData.VLANID))
            {
                Debug.Log($"VLAN {connectionData.VLANID} already assigned to {obj.name}.");
                return;
            }

            // Clear the input field and show the UI
            vlanInputField.text = string.Empty;
            vlanAssignmentUI.SetActive(true);
        }
    }
    // Method to assign VLAN
    public void AssignVlan()
    {
        if (selectedForVlanAssignment == null) return;

        string inputVlan = vlanInputField.text;

        // Check if the input is empty
        if (string.IsNullOrWhiteSpace(inputVlan))
        {
            Debug.LogWarning("VLAN ID input is empty.");
            UpdateTip("Қате: VLAN идентификаторы енгізілмеген.");
            isVLANswrited = true;
            return;
        }

        // Validate VLAN ID (e.g., numeric and within range 1-4096)
        if (int.TryParse(inputVlan, out int vlanID) && vlanID >= 1 && vlanID <= 4096)
        {
            // Check if VLAN ID is already assigned (optional)
            if (assignedVlans.Contains(inputVlan))
            {
                Debug.LogError($"Error: VLAN {inputVlan} is already assigned.");
                UpdateTip($"Қате: VLAN {inputVlan} қазірдің өзінде қолданылады.");
                isVLANswrited = true;
                return;
            }

            // Assign VLAN to the switch
            ConnectionData connectionData = selectedForVlanAssignment.GetComponent<ConnectionData>();
            if (connectionData != null)
            {
                // Remove the previous VLAN if necessary
                if (!string.IsNullOrWhiteSpace(connectionData.VLANID))
                {
                    assignedVlans.Remove(connectionData.VLANID);
                }

                connectionData.VLANID = inputVlan; // Assign VLAN ID as a string
                assignedVlans.Add(inputVlan); // Add VLAN ID to the list
                Debug.Log($"VLAN {inputVlan} assigned to {selectedForVlanAssignment.name}.");
                UpdateTip($"VLAN {inputVlan} нысанға тағайындалды.");
            }
        }
        else
        {
            Debug.LogError("Invalid VLAN ID.");
            UpdateTip("Қате: дұрыс емес VLAN идентификаторы.");
        }

        // Hide the UI and reset selection
        //vlanAssignmentUI.SetActive(false);
        isVLANswrited = false;
        selectedForVlanAssignment = null;
    }
    // Метод для открытия UI назначения IP
    public void OpenIpAssignmentUI(GameObject obj)
    {
        selectedForIpAssignment = obj;

        // Проверяем, есть ли уже назначенный IP
        ConnectionData connectionData = obj.GetComponent<ConnectionData>();
        if (connectionData != null && !string.IsNullOrEmpty(connectionData.IPAddress))
        {
            return;
        }

        // Очищаем поле, если IP отсутствует
        ipInputField.text = string.Empty;

        // Показываем UI
        ipAssignmentUI.SetActive(true);
    }
    // Метод для подтверждения назначения IP
    public void AssignIpAddress()
    {
        if (selectedForIpAssignment == null) return;

        string inputIp = ipInputField.text;

        // Проверяем, пусто ли поле ввода
        if (string.IsNullOrWhiteSpace(inputIp))
        {
            Debug.LogWarning("Поле ввода IP-адреса пусто.");
            UpdateTip("Қате: IP-адрес енгізілмеген.");
            isIPadresswrited = true;
            return;
        }

        // Валидация IP-адреса
        if (IsValidIp(inputIp))
        {
            // Проверка на уже используемый IP
            if (assignedIps.Contains(inputIp))
            {
                Debug.LogError($"Ошибка: IP {inputIp} уже используется.");
                UpdateTip($"Қате: IP {inputIp} қазірдің өзінде қолданылады.");
                isIPadresswrited = true;
                return;
            }

            // Удаляем предыдущий IP объекта, если он был
            ConnectionData connectionData = selectedForIpAssignment.GetComponent<ConnectionData>();
            if (connectionData != null)
            {
                if (!string.IsNullOrEmpty(connectionData.IPAddress))
                {
                    assignedIps.Remove(connectionData.IPAddress);
                }

                connectionData.IPAddress = inputIp; // Назначаем IP
                assignedIps.Add(inputIp); // Добавляем IP в список занятых
                ipToComputerMap.Add(inputIp, selectedForIpAssignment); // Добавляем IP в список занятых
                Debug.Log($"IP {inputIp} assigned to {selectedForIpAssignment.name}.");
                UpdateTip($"IP {inputIp} нысанға тағайындалды.");
            }
        }
        else
        {
            Debug.LogError("Неверный IP-адрес.");
            UpdateTip("Қате: дұрыс емес IP-адрес.");
            isIPadresswrited = false;
        }
        //ipAssignmentUI.SetActive(false); // Закрываем UI
        isIPadresswrited = false;
        selectedForIpAssignment = null;
    }
    // Метод проверки валидности IP-адреса
    private bool IsValidIp(string ip)
    {
        string[] parts = ip.Split('.');
        if (parts.Length != 4) return false;

        foreach (string part in parts)
        {
            if (!int.TryParse(part, out int num) || num < 0 || num > 255)
            {
                return false;
            }
        }

        return true;
    }
    void CreateConnectionLine()
    {
        GameObject lineObject = Instantiate(linePrefab);
        LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, firstObject.transform.position);
        lineRenderer.SetPosition(1, secondObject.transform.position);
    }
    private void AddConnection(GameObject fromObject, GameObject toObject)
    {
        if (fromObject.CompareTag("Switch"))
        {
            if (!switchConnections.ContainsKey(fromObject))
            {
                switchConnections[fromObject] = new List<GameObject>();
            }
            if (!switchConnections[fromObject].Contains(toObject))
            {
                switchConnections[fromObject].Add(toObject);
                Debug.Log($"Добавлено подключение: {fromObject.name} -> {toObject.name}");
            }
            PrintConnections(fromObject);
        }
    }
    // Метод для вывода подключений (например, по кнопке)
    public void PrintConnections(GameObject switchObject)
    {
        if (switchConnections.ContainsKey(switchObject))
        {
            Debug.Log($"Подключенные устройства к {switchObject.name}:");
            foreach (var connectedDevice in switchConnections[switchObject])
            {
                Debug.Log($"- {connectedDevice.name}");
            }
        }
        else
        {
            Debug.Log($"Нет подключений для {switchObject.name}");
        }
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
