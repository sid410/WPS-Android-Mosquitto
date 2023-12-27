using UnityEngine;

public class DistanceCalculator : MonoBehaviour
{
    [SerializeField]
    private CalibrationConstants calibrationConstants;

    public float CalculateDistance(int deviceID, float rssi)
    {
        if (deviceID < 0 || deviceID >= calibrationConstants.referenceRSSI.Length)
        {
            Debug.LogError("Invalid device ID");
            return -1f;
        }

        // Using the Log-Distance Path Loss Model
        float distance = Mathf.Pow(10, ((calibrationConstants.referenceRSSI[deviceID] - rssi) / (10 * calibrationConstants.pathLossExponent[deviceID])));

        return distance;
    }

    void Start()
    {
        int deviceID = 1;
        float rssiValue = -84f;
        float distance = CalculateDistance(deviceID, rssiValue);

        Debug.Log($"Distance from Device {deviceID}: {distance} meters");
    }
}
