using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EspPositions", menuName = "CustomEsp/EspPositions")]
public class EspPositions : ScriptableObject
{
    // (x, y) Positions for each ESP
    public Vector2[] pixelCoordinates;
}