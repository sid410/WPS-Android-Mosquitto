using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PlaneCoordinatesMapper : MonoBehaviour
{
    [SerializeField]
    private GameObject personVisualization;

    [SerializeField]
    private DistanceCalculator distanceCalculator;

    [SerializeField]
    private EspPositions espPos;

    private Dictionary<int, float> accesspointDistances = new Dictionary<int, float>();

    private void Start()
    {
        // need to have pixel coordinate of esp 1 and 2 (element 0 and 1)
        // plus the gap between them in meters, set in EspPositions ScriptableObject
        espPos.CalibrateMeterPixelRatio();
    }

    public Vector3 MapToLocalPlane(Vector2 input)
    {
        Vector3 output = Vector3.zero;

        output.x = Mathf.Lerp(5, -5, Mathf.InverseLerp(0, 500, input.x));
        output.y = 0;
        output.z = Mathf.Lerp(-5, 5, Mathf.InverseLerp(0, 500, input.y));

        return output;
    }

    public void UpdateLocationVisualization(Dictionary<int, float> nearbyAPs)
    {
        accesspointDistances.Clear();

        foreach (var ap in nearbyAPs) 
        {
            float dist = distanceCalculator.CalculateDistance(ap.Key, ap.Value);
            accesspointDistances[ap.Key] = dist;
        }

        // we cannot predict the location if there is less than 2 points
        if (accesspointDistances.Count < 2)
        {
            personVisualization.SetActive(false);
            return;
        }
        else personVisualization.SetActive(true);

        FindPersonMapLocation(accesspointDistances);
    }

    private void FindPersonMapLocation(Dictionary<int, float> dictionary)
    {
        var orderedDictionary = dictionary.OrderBy(kvp => kvp.Value);
        var leastValues = orderedDictionary.Take(2).ToArray();

        float leastValue = leastValues[0].Value;
        float secondLeastValue = leastValues[1].Value;

        float meterGap = espPos.meterPixelRatio * Vector2.Distance(espPos.pixelCoordinates[leastValues[0].Key], 
                                                                    espPos.pixelCoordinates[leastValues[1].Key]);

        Vector2 twoDimLoc = InterpolatePoints(espPos.pixelCoordinates[leastValues[0].Key],
                                                espPos.pixelCoordinates[leastValues[1].Key],
                                                leastValues[0].Value / meterGap);

        Vector3 threeDimLoc = MapToLocalPlane(twoDimLoc);
        threeDimLoc.y += 2f; // put as 2nd layer

        personVisualization.transform.localPosition = threeDimLoc;
    }

    private Vector2 InterpolatePoints(Vector2 point1, Vector2 point2, float percent)
    {
        percent = Mathf.Clamp01(percent);

        float x = Mathf.Lerp(point1.x, point2.x, percent);
        float y = Mathf.Lerp(point1.y, point2.y, percent);

        return new Vector2(x, y);
    }
}
