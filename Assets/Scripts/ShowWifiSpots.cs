using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShowWifiSpots : MonoBehaviour
{
    [SerializeField]
    private EspPositions espPos;

    [SerializeField]
    private PlaneCoordinatesMapper planeMapper;

    [SerializeField]
    private GameObject wifiVizPrefab;
    private GameObject[] wifiVisualization;

    private List<int> activeWifis, visualizedWifis, wifiToSpawn, wifiToDestroy;
    
    private void Start()
    {
        wifiVisualization = new GameObject[espPos.pixelCoordinates.Length];

        activeWifis = new List<int>();
        visualizedWifis = new List<int>();
        wifiToSpawn = new List<int>();
        wifiToDestroy = new List<int>();

        //StartCoroutine(InjectTestData());
    }

    public void UpdateWifiVisualizations(Dictionary<int, float> apDictionary)
    {
        if(apDictionary.Count < 1) return;

        // clear the list contents
        activeWifis.Clear();
        wifiToSpawn.Clear();
        wifiToDestroy.Clear();

        foreach (var apData in apDictionary)
        {
            activeWifis.Add(apData.Key);
        }

        wifiToSpawn = activeWifis.Except(visualizedWifis).ToList();
        wifiToDestroy = visualizedWifis.Except(activeWifis).ToList();

        SpawnWifiObjects(wifiToSpawn);
        DestroyWifiObjects(wifiToDestroy);

        // prepare this list for comparison of next call
        visualizedWifis.Clear();
        visualizedWifis = activeWifis.ToList();
    }

    private void SpawnWifiObjects(List<int> ids)
    {
        foreach (var id in ids)
        {
            wifiVisualization[id] = Instantiate(wifiVizPrefab);
            wifiVisualization[id].gameObject.name = $"WifiSpot{id}";

            wifiVisualization[id].transform.parent = transform;
            wifiVisualization[id].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            wifiVisualization[id].transform.localEulerAngles = new Vector3(90f, 0.2f, 180f);

            Vector3 mapLocation = planeMapper.MapToLocalPlane(espPos.pixelCoordinates[id]);
            wifiVisualization[id].transform.localPosition = new Vector3(mapLocation.x, 0f, mapLocation.z - 0.2f);
        }
    }

    private void DestroyWifiObjects(List<int> ids)
    {
        foreach (var id in ids)
        {
            Destroy(wifiVisualization[id]);
        }
    }

    private IEnumerator InjectTestData()
    {
        Dictionary<int, float> testData = new Dictionary<int, float>();
        testData.Add(0, 0);
        testData.Add(1, 0);
        testData.Add(2, 0);

        UpdateWifiVisualizations(testData);
        yield return new WaitForSeconds(3);

        testData.Remove(1);
        testData.Remove(2);
        UpdateWifiVisualizations(testData);
        yield return new WaitForSeconds(3);

        testData.Remove(0);
        testData.Add(1, 0);
        UpdateWifiVisualizations(testData);
        yield return new WaitForSeconds(3);

        testData.Remove(1);
        testData.Add(2, 0);
        UpdateWifiVisualizations(testData);
        yield return new WaitForSeconds(3);

        testData.Remove(2);
        testData.Add(0, 0);
        UpdateWifiVisualizations(testData);
        Debug.Log("Testing ended");
    }
}
