using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EspPositions", menuName = "CustomEsp/EspPositions")]
public class EspPositions : ScriptableObject
{
    public float firstTwoEspGap; // the gap between ESP 1 and 2, in meters
    public float meterPixelRatio; // used for meter to pixel conversion

    public Vector2[] pixelCoordinates; // (x, y) Positions for each ESP

    public void CalibrateMeterPixelRatio()
    {
        meterPixelRatio = firstTwoEspGap / Vector2.Distance(pixelCoordinates[0], pixelCoordinates[1]);
    }
}