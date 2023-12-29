using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Collections;

public class PlaneCoordinatesMapper : MonoBehaviour
{
    [SerializeField]
    private GameObject personVisualization;

    [SerializeField]
    private Sprite idleSprite;
    private SpriteRenderer personSprite;
    private Animator personWalkingAnim;
    private bool isPersonLerping;

    [SerializeField]
    private MqttMessageHandler mqttMessageHandler;

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

        personSprite = personVisualization.GetComponent<SpriteRenderer>();
        personWalkingAnim = personVisualization.GetComponent<Animator>();
        isPersonLerping = false;
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
            personWalkingAnim.enabled = false;
            personSprite.sprite = idleSprite;
            //personVisualization.SetActive(false);
            return;
        }
        else personWalkingAnim.enabled = true;

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

        // publish to Node-red server
        PublishLocationToServer(twoDimLoc);

        Vector3 threeDimLoc = MapToLocalPlane(twoDimLoc);
        threeDimLoc.z -= 0.3f; // offset to up a little

        //personVisualization.transform.localPosition = threeDimLoc;
        StartCoroutine(LerpPosition(personVisualization, threeDimLoc, 0.2f));
    }

    private Vector2 InterpolatePoints(Vector2 point1, Vector2 point2, float percent)
    {
        percent = Mathf.Clamp01(percent);

        float x = Mathf.Lerp(point1.x, point2.x, percent);
        float y = Mathf.Lerp(point1.y, point2.y, percent);

        return new Vector2(x, y);
    }

    private void PublishLocationToServer(Vector2 personMapLocation)
    {
        string strLocation = $"x: {personMapLocation.x}, y: {personMapLocation.y}";
        mqttMessageHandler.SendBrokerMessage($"DRR/Map/{SystemInfo.deviceUniqueIdentifier}/pos", strLocation);
    }

    private IEnumerator LerpPosition(GameObject objectToLerp, Vector3 targetPosition, float lerpDuration)
    {
        if (isPersonLerping) yield break;
        isPersonLerping = true;

        float timeElapsed = 0f;
        Vector3 initialPosition = objectToLerp.transform.localPosition;

        while (timeElapsed < lerpDuration)
        {
            objectToLerp.transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        objectToLerp.transform.localPosition = targetPosition;
        isPersonLerping = false;
    }
}
