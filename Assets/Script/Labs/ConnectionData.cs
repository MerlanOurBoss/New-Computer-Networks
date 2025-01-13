using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ConnectionData : MonoBehaviour
{
    public Toggle SelectedPort;
    public string IPAddress;
    public TMP_Text IPAddressText; 

    private void Update()
    {
        IPAddressText.text = IPAddress;
    }
}
