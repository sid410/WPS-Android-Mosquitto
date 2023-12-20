using System.Net;
using UnityEngine;

public class PlaneCoordinatesMapper : MonoBehaviour
{
    [Range(0f, 500f)]
    public float myLocationX, myLocationY;

    public GameObject myLocationVisualization;
    
    // just temporary to debug. remove update loop later
    // when the message callback is ready.
    private void Update()
    {
        if (myLocationVisualization == null) return;

        myLocationVisualization.transform.localPosition = MapToLocalPlane(new Vector2(myLocationX, myLocationY));
    }

    public Vector3 MapToLocalPlane(Vector2 input)
    {
        Vector3 output = Vector3.zero;

        output.x = Mathf.Lerp(5, -5, Mathf.InverseLerp(0, 500, input.x));
        output.y = 0;
        output.z = Mathf.Lerp(-5, 5, Mathf.InverseLerp(0, 500, input.y));

        return output;
    }
}
