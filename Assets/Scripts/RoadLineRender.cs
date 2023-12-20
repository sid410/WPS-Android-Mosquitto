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
        RoadSetColor(Color.green);
    }

    private void RoadRenderInit()
    {
        Vector3 startPoint = planeCoordinatesMapper.MapToLocalPlane(startRoad);
        Vector3 endPoint = planeCoordinatesMapper.MapToLocalPlane(endRoad);

        startPoint.y += 1f;
        endPoint.y += 1f;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }

    public void RoadSetColor(Color setColor)
    {
        lineRenderer.material.color = setColor;
    }
}
