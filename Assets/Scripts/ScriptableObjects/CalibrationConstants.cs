using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CalibrationConstants", menuName = "CustomEsp/CalibrationConstants")]
public class CalibrationConstants : ScriptableObject
{
    // Calibration constants for each ESP
    public float[] referenceRSSI; // RSSI at the reference distance (1 meter)
    public float[] pathLossExponent; // Path loss exponent for each device

    public void Initialize(int numEsp)
    {
        referenceRSSI = new float[numEsp];
        pathLossExponent = new float[numEsp];
    }
}