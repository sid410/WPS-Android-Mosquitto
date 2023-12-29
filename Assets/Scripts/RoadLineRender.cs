using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadLineRender : MonoBehaviour
{
    [SerializeField]
    private Vector2 startRoad;
    [SerializeField]
    private Vector2 endRoad;

    private LineRenderer lineRenderer;
    public PlaneCoordinatesMapper planeCoordinatesMapper;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        RoadRenderInit();
        SetColor(Color.green);
    }

    private void RoadRenderInit()
    {
        Vector3 startPoint = planeCoordinatesMapper.MapToLocalPlane(startRoad);
        Vector3 endPoint = planeCoordinatesMapper.MapToLocalPlane(endRoad);

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }

    private void SetColor(Color setColor)
    {
        lineRenderer.material.color = setColor;
    }

    public void RoadSetColor(string strColor)
    {
        switch (strColor)
        {
            case "green":
                SetColor(Color.green);
                break;
            case "yellow":
                SetColor(Color.yellow);
                break;
            case "red":
                SetColor(Color.red);
                break;
            default:
                Debug.Log("Invalid color");
                break;
        }
    }
}
