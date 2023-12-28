//#define EDITING

#if EDITING || (UNITY_ANDROID && !UNITY_EDITOR)
#define PLATFORM_SUPPORTED
#endif

using System.Collections.Generic;
using UnityEngine;
using System;

namespace FSG.Android.Wifi
{
    /// <summary>
    /// Handles interop between Unity and the WifiManager Android class
    /// </summary>
    public static class AndroidWifiManager
    {
        public enum ConnectResult
        {
            SUCCESS,
            ADD_NETWORK_FAILED,
            DISCONNECT_FAILED,
            ENABLE_NETWORK_FAILED,
            CONNECT_FAILED,
            UNSUPPORTED_PLATFORM,
        }
#if UNITY_EDITOR
        private static string s_debugConnectedNetwork = string.Empty;
        private static bool s_debugWifiEnabled = true;
#endif

        #region Private Methods
#if PLATFORM_SUPPORTED
        /// <summary>
        /// Returns the Unity applications activity
        /// </summary>
        private static AndroidJavaObject GetActivity()
        {
            try
            {
                return new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                Debug.LogError("Error getting currentActivity, are you sure you're on Android?");
                return null;
            }
        }

        /// <summary>
        /// Returns the WifiManager object from the Unity activity
        /// </summary>
        private static AndroidJavaObject GetWiFiManager(AndroidJavaObject activity)
        {
            try
            {
                CheckPermissions();
                return activity.Call<AndroidJavaObject>("getSystemService", "wifi");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                Debug.LogError("Error getting wifi service, are you sure you're on Android?");
                return null;
            }
        }
#if UNITY_2019_2_OR_NEWER
        private static string[] s_requiredPermissions = new string[]
        {
            UnityEngine.Android.Permission.CoarseLocation,
            UnityEngine.Android.Permission.FineLocation
        };
        /// <summary>
        /// Ensure the required permissions are granted
        /// </summary>
        private static void CheckPermissions()
        {
            for (int i = 0; i < s_requiredPermissions.Length; i++)
            {
                if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(s_requiredPermissions[i]))
                {
                    UnityEngine.Android.Permission.RequestUserPermission(s_requiredPermissions[i]);
                }
            }
        }
#else
        private static void CheckPermissions() { }
#endif

        /// <summary>
        /// Converts a C# string array to a java.lang.String array
        /// </summary>
        /// <param name="values">Array to be converted</param>
        /// <returns>The java array</returns>
        private static AndroidJavaObject StringArrayToJavaArray(string[] values)
        {
            AndroidJavaClass arrayClass = new AndroidJavaClass("java.lang.reflect.Array");
            AndroidJavaObject arrayObject = arrayClass.CallStatic<AndroidJavaObject>("newInstance", new AndroidJavaClass("java.lang.String"), values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                arrayClass.CallStatic("set", arrayObject, i, values[i].ToJavaString());
            }
            return arrayObject;
        }
#endif
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the built SDK version
        /// </summary>
        /// <returns>Version code of the current Android SDK</returns>
        public static int GetAndroidBuildVersion()
        {
            try
            {
                using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
                {
                    return version.GetStatic<int>("SDK_INT");
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            return -1;
        }

        /// <summary>
        /// Enable or disable Wi-Fi.
        /// </summary>
        /// <param name="enabled">True to enable, false to disable.</param>
        /// <returns>False if the request cannot be satisfied; true indicates that wifi is either already in the requested state, or in progress toward the requested state.</returns>
        public static bool SetWifiEnabled(bool enabled)
        {
#if PLATFORM_SUPPORTED
            try
            {
                using (var activity = GetActivity())
                using (var wifiManager = GetWiFiManager(activity))
                {
                    return wifiManager.Call<bool>("setWifiEnabled", enabled);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
#elif UNITY_EDITOR
            s_debugWifiEnabled = enabled;
            if (!enabled)
            {
                s_debugConnectedNetwork = string.Empty;
            }
#endif
            return false;
        }

        /// <summary>
        /// Return whether Wi-Fi is enabled or disabled.
        /// </summary>
        public static bool IsWifiEnabled()
        {
#if PLATFORM_SUPPORTED
            try
            {
                using (var activity = GetActivity())
                using (var wifiManager = GetWiFiManager(activity))
                {
                    return wifiManager.Call<bool>("isWifiEnabled");
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return false;
#elif UNITY_EDITOR
            return s_debugWifiEnabled;
#else
            return false;
#endif
        }

        /// <summary>
        /// Begins a scan to find available wifi networks. Limited to 4 times per 2 minutes while the app is open.
        /// </summary>
        /// <returns>True if the operation succeeded, i.e., the scan was initiated.</returns>
        public static bool StartScan()
        {
#if PLATFORM_SUPPORTED
            try
            {
                using (var activity = GetActivity())
                using (var wifiManager = GetWiFiManager(activity))
                {
                    return wifiManager.Call<bool>("startScan");
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
#endif
            return false;
        }

        /// <summary>
        /// Return the results of the latest access point scan.
        /// </summary>
        /// <returns>The list of access points found in the most recent scan. An app must hold ACCESS_COARSE_LOCATION or ACCESS_FINE_LOCATION permission in order to get valid results.</returns>
        public static List<AndroidWifiScanResults> GetScanResults()
        {
            List<AndroidWifiScanResults> results = new List<AndroidWifiScanResults>();
#if PLATFORM_SUPPORTED
            try
            {
                using (var activity = GetActivity())
                using (var wifiManager = GetWiFiManager(activity))
                {
                    using (var androidList = wifiManager.Call<AndroidJavaObject>("getScanResults"))
                    {
                        int size = androidList.Call<int>("size");
                        for (int i = 0; i < size; i++)
                        {
                            var result = AndroidWifiScanResults.FromAndroidObject(androidList.Call<AndroidJavaObject>("get", i));
                            results.Add(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
#elif UNITY_EDITOR
            if (s_debugWifiEnabled)
            {
                results.Add(new AndroidWifiScanResults()
                {
                    SSID = "DRR_ESP1",
                    securityType = AndroidWifiSecurityType.OPEN,
                    level = -77,
                });
                results.Add(new AndroidWifiScanResults()
                {
                    SSID = "DRR_ESP2",
                    securityType = AndroidWifiSecurityType.WEP,
                    level = -83,
                });
                results.Add(new AndroidWifiScanResults()
                {
                    SSID = "DRR_ESP3",
                    securityType = AndroidWifiSecurityType.WPA,
                    level = -100,
                });
            }
#endif
            return results;
        }

        /// <summary>
        /// Return a list of all the networks configured for the current foreground user. 
        /// </summary>
        /// <returns>A list of network configurations</returns>
        public static List<AndroidWifiConfiguration> GetConfiguredNetworks()
        {
            List<AndroidWifiConfiguration> results = new List<AndroidWifiConfiguration>();
#if PLATFORM_SUPPORTED
            try
            {
                using (var activity = GetActivity())
                using (var wifiManager = GetWiFiManager(activity))
                {
                    using (var androidList = wifiManager.Call<AndroidJavaObject>("getConfiguredNetworks"))
                    {
                        int size = androidList.Call<int>("size");
                        for (int i = 0; i < size; i++)
                        {
                            var result = AndroidWifiConfiguration.FromAndroidObject(androidList.Call<AndroidJavaObject>("get", i));
                            results.Add(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
#elif UNITY_EDITOR
            if (!string.IsNullOrEmpty(s_debugConnectedNetwork))
            {
                results.Add(new AndroidWifiConfiguration()
                {
                    SSID = s_debugConnectedNetwork,
                    status = AndroidWifiConfiguration.Status.CURRENT,
                });
            }
#endif
            return results;
        }

        /// <summary>
        /// Gets the Wi-Fi enabled state.
        /// </summary>
        public static AndroidWifiState GetWifiState()
        {
#if PLATFORM_SUPPORTED
            try
            {
                using (var activity = GetActivity())
                using (var wifiManager = GetWiFiManager(activity))
                {
                    return (AndroidWifiState)wifiManager.Call<int>("getWifiState");
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return AndroidWifiState.UNKNOWN;
#elif UNITY_EDITOR
            return s_debugWifiEnabled ? AndroidWifiState.ENABLED : AndroidWifiState.DISABLED;
#else
            return AndroidWifiState.UNKNOWN;
#endif
        }

        /// <summary>
        /// Add a new network description to the set of configured networks. The networkId field of the supplied configuration object is ignored.
        ///  The new network will be marked DISABLED by default. To enable it, called enableNetwork
        /// </summary>
        /// <param name="wifiConfiguration">The set of variables that describe the configuration</param>
        /// <returns>The ID of the newly created network description. This is used in other operations to specified the network to be acted upon. Returns -1 on failure.</returns>
        public static int AddNetwork(AndroidWifiConfiguration wifiConfiguration)
        {
#if PLATFORM_SUPPORTED
            try
            {
                using (var activity = GetActivity())
                using (var wifiManager = GetWiFiManager(activity))
                {
                    var configurations = GetConfiguredNetworks();
                    int index = 0;
                    for (int i = 0; i < configurations.Count; i++)
                    {
                        if (configurations[i].networkId == wifiConfiguration.networkId)
                        {
                            index = i;
                            break;
                        }
                    }
                    using (var androidList = wifiManager.Call<AndroidJavaObject>("getConfiguredNetworks"))
                    {
                        return wifiManager.Call<int>("addNetwork", androidList.Call<AndroidJavaObject>("get", index));
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
#endif
            return -1;
        }

        /// <summary>
        /// Remove the specified network from the list of configured networks
        /// </summary>
        /// <param name="networkId">The ID of the network as returned by AddNetwork() or GetConfiguredNetworks().</param>
        /// <returns>True if the operation succeeded</returns>
        public static bool RemoveNetwork(int networkId)
        {
#if PLATFORM_SUPPORTED
            try
            {
                using (var activity = GetActivity())
                using (var wifiManager = GetWiFiManager(activity))
                {
                    wifiManager.Call<bool>("disconnect");
                    return wifiManager.Call<bool>("removeNetwork", networkId);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
#endif
            return false;
        }

        /// <summary>
        /// Allow a previously configured network to be associated with. If attemptConnect is true, an attempt to connect to the selected network is initiated.
        /// </summary>
        /// <param name="networkId">The ID of the network as returned by AddNetwork() or GetConfiguredNetworks().</param>
        /// <param name="attemptConnect">Initate a connection to the network</param>
        /// <returns>True if the operation succeeded</returns>
        public static bool EnableNetwork(int networkId, bool attemptConnect)
        {
#if PLATFORM_SUPPORTED
            try
            {
                using (var activity = GetActivity())
                using (var wifiManager = GetWiFiManager(activity))
                {
                    return wifiManager.Call<bool>("enableNetwork", networkId, attemptConnect);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
#endif
            return false;
        }

        /// <summary>
        /// Disables a network based off of it's networkId
        /// </summary>
        /// <param name="networkId">The ID of the network as returned by AddNetwork() or GetConfiguredNetworks().</param>
        /// <returns>True if the operation suceeded</returns>
        public static bool DisableNetwork(int networkId)
        {
#if PLATFORM_SUPPORTED
            try
            {
                using (var activity = GetActivity())
                using (var wifiManager = GetWiFiManager(activity))
                {
                    return wifiManager.Call<bool>("disableNetwork", networkId);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
#endif
            return false;
        }

        /// <summary>
        /// Disconnects from the currently connected to network
        /// </summary>
        /// <returns>True if the operation succeeded</returns>
        public static bool Disconnect()
        {
#if PLATFORM_SUPPORTED
            try
            {
                using (var activity = GetActivity())
                using (var wifiManager = GetWiFiManager(activity))
                {
                    return wifiManager.Call<bool>("disconnect");
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return false;
#elif UNITY_EDITOR
            s_debugConnectedNetwork = string.Empty;
            return true;
#else
            return false;
#endif
        }

        /// <summary>
        /// Connect to the specified SSID using the supplied password.
        /// </summary>
        /// <param name="ssid">The network SSID obtained through GetScanResults or GetConfiguredNetworks</param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static ConnectResult Connect(string ssid, string password = "")
        {
#if PLATFORM_SUPPORTED
            try
            {
                AndroidWifiConfiguration existing = GetConfiguredNetworks().Find(q => q.SSID == ssid);
                AndroidWifiScanResults scanResult = GetScanResults().Find(q => q.SSID == ssid);
                using (var activity = GetActivity())
                using (var wifiManager = GetWiFiManager(activity))
                {
                    using (var wifiConfig = new AndroidJavaObject("android.net.wifi.WifiConfiguration"))
                    {
                        wifiConfig.Set("SSID", string.Format("\"{0}\"", ssid).ToJavaString());
                        // determine the security type of the network
                        AndroidWifiSecurityType securityType = AndroidWifiSecurityType.WPA2;
                        if (scanResult != null)
                        {
                            securityType = scanResult.securityType;
                        }
                        else if (string.IsNullOrEmpty(password))
                        {
                            securityType = AndroidWifiSecurityType.OPEN;
                        }
                        switch (securityType)
                        {
                            case AndroidWifiSecurityType.WPA:
                            case AndroidWifiSecurityType.WPA2:
                                {
                                    wifiConfig.Set("preSharedKey", string.Format("\"{0}\"", password).ToJavaString());
                                    break;
                                }
                            case AndroidWifiSecurityType.WEP:
                                {
                                    try
                                    {
                                        password = string.Format("\"{0}\"", password);
                                        wifiConfig.Set("wepTxKeyIndex", 0);
                                        wifiConfig.Set("wepKeys", StringArrayToJavaArray(new string[] { password }));
                                        using (var groupCipher = new AndroidJavaObject("java.util.BitSet"))
                                        {
                                            groupCipher.Call("set", 0);
                                            wifiConfig.Set("allowedGroupCiphers", groupCipher);
                                        }
                                        using (var allowedKey = new AndroidJavaObject("java.util.BitSet"))
                                        {
                                            allowedKey.Call("set", 0);
                                            wifiConfig.Set("allowedKeyManagement", allowedKey);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.LogError("Error connecting to WEP network. It's possible it was deprectated in Android API 28.");
                                        Debug.LogException(ex);
                                        return ConnectResult.UNSUPPORTED_PLATFORM;
                                    }
                                    break;
                                }
                            case AndroidWifiSecurityType.OPEN:
                                {
                                    using (var allowedKey = new AndroidJavaObject("java.util.BitSet"))
                                    {
                                        allowedKey.Call("set", 0);
                                        wifiConfig.Set("allowedKeyManagement", allowedKey);
                                    }
                                    break;
                                }
                        }
                        if (existing != null)
                        {
                            wifiManager.Call<bool>("removeNetwork", existing.networkId);
                        }
                        int networkId = wifiManager.Call<int>("addNetwork", wifiConfig);
                        if (networkId < 0)
                        {
                            return ConnectResult.ADD_NETWORK_FAILED;
                        }
                        if (!wifiManager.Call<bool>("disconnect"))
                        {
                            return ConnectResult.DISCONNECT_FAILED;
                        }
                        if (!wifiManager.Call<bool>("enableNetwork", networkId, true))
                        {
                            return ConnectResult.ENABLE_NETWORK_FAILED;
                        }
                        if (!wifiManager.Call<bool>("reconnect"))
                        {
                            return ConnectResult.CONNECT_FAILED;
                        }
                        return ConnectResult.SUCCESS;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return ConnectResult.CONNECT_FAILED;
            }
#elif UNITY_EDITOR
            s_debugConnectedNetwork = ssid;
            return ConnectResult.SUCCESS;
#else
            return ConnectResult.UNSUPPORTED_PLATFORM;
#endif
        }
        #endregion
    }
}