using UnityEngine;

namespace FSG.Android.Wifi
{
    /// <summary>
    /// C# representation of the WifiConfiguration Android class
    /// </summary>
    public class AndroidWifiConfiguration
    {
        /// <summary>
        /// Possible status of a network configuration.
        /// </summary>
        public enum Status
        {
            CURRENT = 0,
            DISABLED = 1,
            ENABLED = 2
        }

        // When set, this network configuration entry should only be used when associating with the AP having the specified BSSID.
        public string BSSID;
        // Fully qualified domain name of a Passpoint configuration
        public string FQDN;
        // The network's SSID.
        public string SSID;
        // This is a network that does not broadcast its SSID, so an SSID-specific probe request must be used for scans.
        public bool hiddenSSID;
        // Flag indicating if this network is provided by a home Passpoint provider or a roaming Passpoint provider.
        public int networkId;
        // Pre-shared key for use with WPA-PSK.
        public string preSharedKey;
        // Name of Passpoint credential provider
        public string providerFriendlyName;
        // The current status of this network configuration entry.
        public Status status;

        public static AndroidWifiConfiguration FromAndroidObject(AndroidJavaObject javaObject)
        {
            return new AndroidWifiConfiguration()
            {
                BSSID = javaObject.GetFieldSafe<string>("BSSID"),
                FQDN = javaObject.GetFieldSafe<string>("FQDN"),
                SSID = javaObject.GetFieldSafe<string>("SSID"),
                hiddenSSID = javaObject.GetFieldSafe<bool>("hiddenSSID"),
                networkId = javaObject.GetFieldSafe<int>("networkId"),
                preSharedKey = javaObject.GetFieldSafe<string>("preSharedKey"),
                providerFriendlyName = javaObject.GetFieldSafe<string>("providerFriendlyName"),
                status = (Status)javaObject.GetFieldSafe<int>("status"),
            };
        }
    }
}