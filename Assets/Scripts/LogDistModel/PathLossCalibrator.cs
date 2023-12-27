using UnityEngine;

public class PathLossCalibrator : MonoBehaviour
{
    [SerializeField]
    private CalibrationConstants calibrationConstants;
    [SerializeField]
    private EspData[] espDatas = null;

    private void Start()
    {
        calibrationConstants.Initialize(espDatas.Length);

        for (int id = 0; id < espDatas.Length; id++) 
        {
            SetReferenceRssiValues(id);
            CalibratePathLossExponent(id, espDatas[id].distances, espDatas[id].rssiValues);
        }
    }

    // Do this first before CalibratePathLossExponent to get correct calibration
    private void SetReferenceRssiValues(int deviceID)
    {
        if (espDatas[deviceID].distances[0] != 1f)
        {
            Debug.LogError("Please set first element of distances to 1 meter");
            return;
        }

        calibrationConstants.referenceRSSI[deviceID] = espDatas[deviceID].rssiValues[0];
    }

    // Function to calibrate the path loss exponent
    private void CalibratePathLossExponent(int deviceID, float[] distances, float[] rssiValues)
    {
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
            float y = calibrationConstants.referenceRSSI[deviceID] - rssiValues[i];

            sumX += x;
            sumY += y;
            sumXY += x * y;
            sumX2 += x * x;
        }

        float n = (distances.Length * sumXY - sumX * sumY) / (distances.Length * sumX2 - sumX * sumX);

        // Update path loss exponent for the device
        calibrationConstants.pathLossExponent[deviceID] = n;
    }
}
