using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DRR_ESP", menuName = "CustomEsp/EspData")]
public class EspData : ScriptableObject
{
    // Calibration data for each ESP
    public float[] distances; // the distance where the RSSI sample was taken
    public float[] rssiValues; // the corresponding RSSI sample
}