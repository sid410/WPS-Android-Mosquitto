using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using FSG.Android.Wifi;
using UnityEditor;

public class ScanWifi : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI dictText, buttonText;

    [SerializeField]
    private string WifiNamePattern;

    private Coroutine scanCoroutine;
    private Dictionary<string, int> wifiSignalStrengths = new Dictionary<string, int>();

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
                if (result.SSID.StartsWith(WifiNamePattern))
                {
                    wifiSignalStrengths.Add(result.SSID, result.level);
                }
            }

            ShowDictionaryContents(wifiSignalStrengths);

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

    private void ShowDictionaryContents(Dictionary<string, int> dict)
    {
        dictText.text = "";
        foreach (var entry in dict)
        {
            dictText.text += $"{entry.Key}: {entry.Value}" + "<br>";
        }
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

