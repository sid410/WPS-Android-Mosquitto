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
}
