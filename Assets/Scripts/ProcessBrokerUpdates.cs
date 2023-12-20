using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using TMPro;


public class ProcessBrokerUpdates : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro temperature, humidity, rain, advisory;

    public RoadLineRender left, bottom, center;

    public void UpdateAppInfo(string topic, string message)
    {
        switch (topic)
        {
            case "DRR/Weather/temperature":
                temperature.text = "Temperature: " + message;
                break;
            case "DRR/Weather/humidity":
                humidity.text = "Humidity: " + message;
                break;
            case "DRR/Weather/rain":
                rain.text = "Rain Intensity: " + message;
                break;
            case "DRR/Weather/advisory":
                advisory.text = message;
                break;

            case "DRR/Map/Waterlevel/Road/left":
                left.RoadSetColor(message);
                break;
            case "DRR/Map/Waterlevel/Road/bottom":
                bottom.RoadSetColor(message);
                break;
            case "DRR/Map/Waterlevel/Road/center":
                center.RoadSetColor(message);
                break;

            default:
                Debug.Log("topic to function not defined");
                break;
        }
    }
}
