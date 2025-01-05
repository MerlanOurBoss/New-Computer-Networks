using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject pcPrefab;       // Префаб для PC
    public GameObject routerPrefab;   // Префаб для Router
    public GameObject switchPrefab;   // Префаб для Switch
    public Canvas canvas;             // Ссылка на Canvas
    private GameObject currentObject; // Текущий создаваемый объект

    private bool isPlacingObject = false; // Флаг размещения объекта

    void Update()
    {
        // Если в процессе размещения объекта
        if (isPlacingObject && currentObject != null)
        {
            // Переводим экранные координаты мыши в локальные координаты Canvas
            Vector2 mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out mousePosition
            );

            // Устанавливаем позицию объекта
            currentObject.GetComponent<RectTransform>().anchoredPosition = mousePosition;

            // Завершаем размещение при клике левой кнопкой
            if (Input.GetMouseButtonDown(0))
            {
                // Активируем взаимодействие с кнопкой
                Button button = currentObject.GetComponent<Button>();
                if (button != null)
                {
                    button.interactable = true;
                }

                isPlacingObject = false; // Завершаем размещение
                currentObject = null;
            }
        }
    }

    // Метод для спавна объекта
    public void SpawnObject(string objectType)
    {
        if (isPlacingObject) return; // Если объект уже размещается, игнорируем

        GameObject prefabToSpawn = null;

        // Определяем, какой префаб создавать
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
            // Создаём объект внутри Canvas
            currentObject = Instantiate(prefabToSpawn, canvas.transform);

            // Обнуляем позицию объекта
            RectTransform rectTransform = currentObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;  // Устанавливаем (0, 0)
            rectTransform.localPosition = Vector3.zero;     // Дополнительно сбрасываем Z, если нужно

            // Отключаем взаимодействие с кнопкой
            Button button = currentObject.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = false;
            }

            isPlacingObject = true; // Включаем режим размещения
        }
    }
}
