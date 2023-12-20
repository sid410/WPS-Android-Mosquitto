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
            baseClient.RegisterTopicHandler(topic, ReadBrokerMessage);
        }
    }

    private void OnDisable()
    {
        foreach (string topic in topicList)
        {
            baseClient.UnregisterTopicHandler(topic, ReadBrokerMessage);
        }
    }

    public void SendBrokerMessage(string topic, string message)
    {
        baseClient.PublishMessageHandler(topic, message);
    }

    private void ReadBrokerMessage(string topic, string message)
    {
        Debug.Log("topic: " + topic + " message: " + message);
    }

}
