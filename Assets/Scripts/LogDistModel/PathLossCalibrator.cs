using UnityEngine;

public class PathLossCalibrator : MonoBehaviour
{
    // Calibration constants for each device
    public float[] referenceRSSI; // RSSI at the reference distance (1 meter)
    public float[] pathLossExponent; // Path loss exponent for each device

    // Function to calibrate the path loss exponent
    public void CalibratePathLossExponent(int deviceID, float[] distances, float[] rssiValues)
    {
        if (deviceID < 0 || deviceID >= referenceRSSI.Length)
        {
            Debug.LogError("Invalid device ID");
            return;
        }

        // Check if there are enough samples
        if (distances.Length != rssiValues.Length || distances.Length < 2)
        {
            Debug.LogError("Insufficient data for calibration");
            return;
        }

        // Use linear regression to estimate path loss exponent
        float sumX = 0f;
        float sumY = 0f;
        float sumXY = 0f;
        float sumX2 = 0f;

        for (int i = 0; i < distances.Length; i++)
        {
            float x = 10 * Mathf.Log10(distances[i]);
            float y = referenceRSSI[deviceID] - rssiValues[i];

            sumX += x;
            sumY += y;
            sumXY += x * y;
            sumX2 += x * x;
        }

        float n = (distances.Length * sumXY - sumX * sumY) / (distances.Length * sumX2 - sumX * sumX);

        // Update path loss exponent for the device
        pathLossExponent[deviceID] = n;

        Debug.Log($"Calibrated Path Loss Exponent for Device {deviceID}: {pathLossExponent[deviceID]}");
    }

    void Start()
    {
        int deviceID = 0;
        float[] distances = { 1f, 30f };
        float[] rssiValues = { -40f, -90f };

        CalibratePathLossExponent(deviceID, distances, rssiValues);
    }
}
