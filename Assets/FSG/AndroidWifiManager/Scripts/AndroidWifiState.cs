namespace FSG.Android.Wifi
{
    /// <summary>
    /// The current state of the device's wifi
    /// </summary>
    public enum AndroidWifiState
    {
        DISABLING = 0,
        DISABLED = 1,
        ENABLING = 2,
        ENABLED = 3,
        UNKNOWN = 4
    }
}