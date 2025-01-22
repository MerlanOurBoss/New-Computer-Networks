using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CableColorOrder : MonoBehaviour
{
    public TMP_Dropdown standardDropdown; // Dropdown ��� ������ ���������
    public TMP_Dropdown[] colorDropdowns; // ���� ����� ��� ��������� ������
    public TMP_Text resultText; // ����� ��� ����������� ���������� ��������

    public bool isCorrect = false;

    // ����� ��� T-568A
    private readonly string[] T568AColors = new string[]
    {
        "������-�����",
        "�������",
        "��������-�����",
        "�����",
        "����-�����",
        "���������",
        "���������-�����",
        "����������"
    };

    // ����� ��� T-568B
    private readonly string[] T568BColors = new string[]
    {
        "��������-�����",
        "���������",
        "������-�����",
        "�����",
        "����-�����",
        "�������",
        "���������-�����",
        "����������"
    };

    private string[] currentStandardColors;

    void Start()
    {
        // ������������� �� ������� ��������� ������ ���������
        standardDropdown.onValueChanged.AddListener(SetCurrentStandard);

        // ������������� ������� �������� � ��������� UI
        SetCurrentStandard(standardDropdown.value);

        // �������������� dropdowns ������
        InitializeColorDropdowns();
    }

    // ����� ��� ��������� �������� ���������
    private void SetCurrentStandard(int standardIndex)
    {
        currentStandardColors = standardIndex == 0 ? T568AColors : T568BColors;
        resultText.text = ""; // ������� ����� ����������

        // ���������� dropdowns ������
        foreach (var dropdown in colorDropdowns)
        {
            dropdown.value = 0;
        }

        // ������������ ����� � ������ dropdown
        InitializeColorDropdowns();
    }

    // ����� ��� ������������� dropdowns ������
    private void InitializeColorDropdowns()
    {
        foreach (var dropdown in colorDropdowns)
        {
            List<string> randomizedColors = T568AColors.OrderBy(c => Random.value).ToList();
            dropdown.ClearOptions();
            dropdown.AddOptions(randomizedColors);
        }
    }

    // ����� ��� �������� ������������ ������� ������
    public void CheckColorOrder()
    {
        for (int i = 0; i < colorDropdowns.Length; i++)
        {
            if (i < currentStandardColors.Length)
            {
                if (!colorDropdowns[i].options[colorDropdowns[i].value].text.Equals(currentStandardColors[i], System.StringComparison.OrdinalIgnoreCase))
                {
                    resultText.text = "������� ������ ��������!";
                    resultText.color = Color.red;
                    isCorrect = false;
                    return;
                }
            }
        }

        resultText.text = "������� ������ ����������!";
        resultText.color = Color.green;
        isCorrect = true;
    }
}
