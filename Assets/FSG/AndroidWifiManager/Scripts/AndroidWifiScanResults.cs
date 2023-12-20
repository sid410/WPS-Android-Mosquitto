using System;
using UnityEngine;

namespace FSG.Android.Wifi
{
    /// <summary>
    /// C# representation of the ScanResults Android class
    /// </summary>
    public class AndroidWifiScanResults
    {
        // The address of the access point.
        public string BSSID;
        // The network name.
        public string SSID;
        // Describes the authentication, key management, and encryption schemes supported by the access point.
        public string capabilities;
        // Not used if the AP bandwidth is 20 MHz If the AP use 40, 80 or 160 MHz, this is the center frequency(in MHz) if the AP use 80 + 80 MHz, this is the center frequency of the first segment(in MHz)
        public int centerFreq0;
        // Only used if the AP bandwidth is 80 + 80 MHz if the AP use 80 + 80 MHz, this is the center frequency of the second segment(in MHz)
        public int centerFreq1;
        // AP Channel bandwidth; one of CHANNEL_WIDTH_20MHZ, CHANNEL_WIDTH_40MHZ, CHANNEL_WIDTH_80MHZ, CHANNEL_WIDTH_160MHZ or CHANNEL_WIDTH_80MHZ_PLUS_MHZ.
        public int channelWidth;
        // The primary 20 MHz frequency(in MHz) of the channel over which the client is communicating with the access point.
        public int frequency;
        // The detected signal level in dBm, also known as the RSSI.
        public int level;
        // Indicates Passpoint operator name published by access point.
        public string operatorFriendlyName;
        // Timestamp in microseconds(since boot) when this result was last seen.
        public long timestamp;
        // Indicates venue name published by access point; only available on Passpoint network and if published by access point.
        public string venueName;
        // The security type of the network.
        public AndroidWifiSecurityType securityType;

        public static AndroidWifiScanResults FromAndroidObject(AndroidJavaObject javaObject)
        {
            var scanResult = new AndroidWifiScanResults()
            {
                BSSID = javaObject.GetFieldSafe<string>("BSSID"),
                SSID = javaObject.GetFieldSafe<string>("SSID"),
                capabilities = javaObject.GetFieldSafe<string>("capabilities"),
                centerFreq0 = javaObject.GetFieldSafe<int>("centerFreq0"),
                centerFreq1 = javaObject.GetFieldSafe<int>("centerFreq1"),
                channelWidth = javaObject.GetFieldSafe<int>("channelWidth"),
                frequency = javaObject.GetFieldSafe<int>("frequency"),
                level = javaObject.GetFieldSafe<int>("level"),
                operatorFriendlyName = javaObject.GetFieldSafe<string>("operatorFriendlyName"),
                timestamp = javaObject.GetFieldSafe<long>("timestamp"),
                venueName = javaObject.GetFieldSafe<string>("venueName"),
            };
            scanResult.securityType = AndroidWifiSecurityType.OPEN;
            string[] securityModes = { "WEP", "WPA", "WPA2" };
            for (int i = securityModes.Length - 1; i >= 0; i--)
            {
                if (scanResult.capabilities.Contains(securityModes[i]))
                {
                    scanResult.securityType = (AndroidWifiSecurityType)Enum.Parse(typeof(AndroidWifiSecurityType), securityModes[i]);
                }
            }
            return scanResult;
        }
    }
}