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

    private string deviceId;

    private void OnEnable()
    {
        baseClient.RegisterTopicHandler("DRR/DeregisterSuccess", QuitApplication);
    }

    private void OnDisable()
    {
        baseClient.UnregisterTopicHandler("DRR/DeregisterSuccess", QuitApplication);
    }

    private void Start()
    {
        deviceId = SystemInfo.deviceUniqueIdentifier;
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

        uidText.text = "UID: " + deviceId;
        mqttMessageHandler.SendBrokerMessage("DRR/RegisterUID", deviceId);
    }

    public void OnQuitButtonPressed()
    {
        mqttMessageHandler.SendBrokerMessage("DRR/DeregisterUID", deviceId);
    }

    private void QuitApplication(string topic, string message)
    {
        if (topic != "DRR/DeregisterSuccess") return;
        if (message != deviceId) return;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
