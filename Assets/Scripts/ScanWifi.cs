using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using FSG.Android.Wifi;
using UnityEditor;
using UnityEngine.Windows;

public class ScanWifi : MonoBehaviour
{
    [SerializeField]
    private PlaneCoordinatesMapper planeMapper;
    [SerializeField]
    private ShowWifiSpots showWifi;

    [SerializeField]
    private TextMeshProUGUI buttonText;

    [SerializeField]
    private string wifiNamePattern;
    public string WifiNamePattern
    {
        get { return wifiNamePattern; }
        set { wifiNamePattern = value; }
    }

    private Coroutine scanCoroutine;
    private Dictionary<string, int> wifiSignalStrengths = new Dictionary<string, int>();
    private Dictionary<int, float> nearbyAccessPoints = new Dictionary<int, float>();

    public enum ScanState
    {
        Scanning, Stopped
    }
    private ScanState State
    {
        get;
        set;
    }

    private void Start()
    {
        State = ScanState.Stopped;
        ToggleScan();
    }

    private IEnumerator ScanWifiNetworks()
    {
        Debug.Log("Started Scanning Wifi");

        while (State == ScanState.Scanning)
        {
            if (AndroidWifiManager.IsWifiEnabled() == false)
            {
                AndroidWifiManager.SetWifiEnabled(true);
                yield return new WaitForSeconds(1);
            }

            AndroidWifiManager.StartScan();
            yield return new WaitForSeconds(1);

            var results = AndroidWifiManager.GetScanResults();

            // regularly clean the dictionary
            wifiSignalStrengths.Clear();

            foreach (AndroidWifiScanResults result in results)
            {
                if (result.SSID.StartsWith(wifiNamePattern))
                {
                    wifiSignalStrengths.Add(result.SSID, result.level);
                }
            }

            //ShowDictionaryContents(wifiSignalStrengths);
            UpdateAccessPointsChanges();

            // In the loop, wait a total of 3 seconds before refresh
            yield return new WaitForSeconds(1);
        }
    }

    private void StopWifiScanning()
    {
        if (scanCoroutine != null)
        {
            StopCoroutine(scanCoroutine);
            Debug.Log("Stopped Scanning Wifi");
        }
    }

    // for debugging
    /*private void ShowDictionaryContents(Dictionary<string, int> dict)
    {
        dictText.text = "";
        foreach (var entry in dict)
        {
            dictText.text += $"{entry.Key}: {entry.Value}" + "<br>";
        }
    }*/

    private void UpdateAccessPointsChanges()
    {
        Dictionary<int, float> accessPoints = GetNearbyAccessPoints();

        planeMapper.UpdateLocationVisualization(accessPoints);
        //showWifi.UpdateWifiVisualizations(accessPoints);
    }

    private int ExtractAccessPointID(string accessPointID)
    {
        if (accessPointID.StartsWith(wifiNamePattern))
        {
            string id = accessPointID.Substring(wifiNamePattern.Length);

            if (int.TryParse(id, out int result))
            {
                return result;
            }
            else
            {
                Debug.LogError("Unable to parse the numeric part as an integer.");
            }
        }
        else
        {
            Debug.LogError("Input string does not start with the expected prefix.");
        }

        return -1;
    }

    public Dictionary<int, float> GetNearbyAccessPoints()
    {
        nearbyAccessPoints.Clear();

        foreach (var wifi in wifiSignalStrengths)
        {
            int id = ExtractAccessPointID(wifi.Key);

            if (id == -1) continue; // skip for invalid IDs

            // remember, DRR_ESP1 is placed as element 0, thus id = 0
            nearbyAccessPoints[id-1] = (float)wifi.Value;
        }
        
        return nearbyAccessPoints;
    }

    public void ToggleScan()
    {
        if (State == ScanState.Scanning)
        {
            State = ScanState.Stopped;
            StopWifiScanning();
        }
        else
        {
            State = ScanState.Scanning;
            scanCoroutine = StartCoroutine(ScanWifiNetworks());
        }

        buttonText.text = State.ToString();
    }
}

