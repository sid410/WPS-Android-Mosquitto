using M2MqttUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeviceBrokerConnection : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro uidText;

    [SerializeField]
    private MqttMessageHandler mqttMessageHandler;
    [SerializeField]
    private BaseClient baseClient;

    private void Start()
    {
        StartCoroutine(RegisterDeviceToBroker());
    }

    private IEnumerator RegisterDeviceToBroker()
    {
        uidText.text = "Connecting...";

        // wait until we have established connection with the broker
        while (!baseClient.Connected)
        {
            yield return null;
        }

        string deviceId = SystemInfo.deviceUniqueIdentifier;
        uidText.text = "UID: " + deviceId;
        mqttMessageHandler.SendBrokerMessage("DRR/RegisterUID", deviceId);
    }
}
