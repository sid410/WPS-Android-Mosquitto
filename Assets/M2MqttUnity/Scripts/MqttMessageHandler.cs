using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M2MqttUnity;

public class MqttMessageHandler : MonoBehaviour
{
    public BaseClient baseClient;

    [SerializeField]
    private string[] topicList;

    private void OnEnable()
    {
        foreach (string topic in topicList)
        {
            baseClient.RegisterTopicHandler(topic, ReadMessage);
        }
    }

    private void OnDisable()
    {
        foreach (string topic in topicList)
        {
            baseClient.UnregisterTopicHandler(topic, ReadMessage);
        }
    }

    public void SendMessage(string topic, string message)
    {
        baseClient.PublishMessageHandler(topic, message);
    }

    private void ReadMessage(string topic, string message)
    {
        Debug.Log("topic: " + topic + " message: " + message);
    }

}
