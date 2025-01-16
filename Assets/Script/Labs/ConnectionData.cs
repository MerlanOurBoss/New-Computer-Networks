using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ConnectionData : MonoBehaviour
{
    public Toggle SelectedPort { get; set; }
    public string IPAddress { get; set; }
    public string VLANID { get; set; }
    public TMP_Text IPAddressText; 
    public TMP_Text VLANText; 

    private void Update()
    {
        IPAddressText.text = IPAddress;
        VLANText.text = VLANID;
    }
}
