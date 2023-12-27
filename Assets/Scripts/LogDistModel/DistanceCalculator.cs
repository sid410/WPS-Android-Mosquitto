using UnityEngine;

public class DistanceCalculator : MonoBehaviour
{
    // Calibration constants for each device
    public float[] referenceRSSI; // RSSI at the reference distance (1 meter)
    public float[] pathLossExponent; // Path loss exponent for each device

    // Function to calculate distance based on RSSI
    public float CalculateDistance(int deviceID, float rssi)
    {
        if (deviceID < 0 || deviceID >= referenceRSSI.Length)
        {
            Debug.LogError("Invalid device ID");
            return -1f;
        }

        // Using the Log-Distance Path Loss Model
        float distance = Mathf.Pow(10, ((referenceRSSI[deviceID] - rssi) / (10 * pathLossExponent[deviceID])));

        return distance;
    }

    // Example usage
    void Start()
    {
        // Example: Calculate distance for Device 0 with RSSI value -70.5
        int deviceID = 0;
        float rssiValue = -85f;
        float distance = CalculateDistance(deviceID, rssiValue);

        Debug.Log($"Distance from Device {deviceID}: {distance} meters");
    }
}
