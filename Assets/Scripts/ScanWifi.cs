using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using FSG.Android.Wifi;

public class ScanWifi : MonoBehaviour
{
    public TextMeshProUGUI scanText, stateText;

    private void Start()
    {
        // Start the scan
        StartCoroutine(PrintWifiNetworks());
    }

    private IEnumerator PrintWifiNetworks()
    {
        while (true)
        {
            // Check if wifi is enabled
            if (AndroidWifiManager.IsWifiEnabled() == false)
            {
                // If not, enable it
                AndroidWifiManager.SetWifiEnabled(true);

                // Give the device time to enable wifi
                yield return new WaitForSeconds(1);
            }

            // Initiate a scan (not always needed)
            AndroidWifiManager.StartScan();

            stateText.text = "StartScan: " + AndroidWifiManager.StartScan().ToString();

            // Wait for the scan to complete
            yield return new WaitForSeconds(1);

            scanText.text = "";

            // Get the list of scan results
            var results = AndroidWifiManager.GetScanResults();
            foreach (AndroidWifiScanResults result in results)
            {
                Debug.LogFormat("SSID: {0} Signal: {1}dBm Security Type: {2}", result.SSID, result.level, result.securityType);
                scanText.text = scanText.text + "SSID: " + result.SSID + " level: " + result.level + "<br>";

            }

            yield return new WaitForSeconds(5);
        }
        
    }
}

