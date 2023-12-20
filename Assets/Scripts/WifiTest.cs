using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System;

namespace FSG.Android.Wifi
{
    public class WifiTest : MonoBehaviour
    {
        private List<AndroidWifiScanResults> m_scanResults = new List<AndroidWifiScanResults>();
        private List<AndroidWifiConfiguration> m_configuredNetworks = new List<AndroidWifiConfiguration>();
        private AndroidWifiState m_currentState = AndroidWifiState.UNKNOWN;
        private GUIStyle m_buttonStyle;
        private GUIStyle m_buttonListStyle;
        private GUIStyle m_textStyle;
        private GUIStyle m_listStyle;
        private GUIStyle m_toggleStyle;
        private GUIStyle m_passwordStyle;
        private Vector2 m_scrollPos;
        private bool m_showOnlyCurrent;
        private AndroidWifiScanResults m_connectingNetwork;
        private bool m_enteringPassword;
        private string m_password;

        public TextMeshProUGUI scanText, stateText;
        private int counter = 0;

        private void Awake()
        {
            AndroidWifiManager.SetWifiEnabled(true);
            m_currentState = AndroidWifiManager.GetWifiState();
            m_configuredNetworks = AndroidWifiManager.GetConfiguredNetworks();

            stateText.text = m_currentState.ToString();

            InvokeRepeating("PeriodicScan", 3.0f, 3.0f);
        }

        private void PeriodicScan()
        {
            AndroidWifiManager.StartScan();
            m_scanResults = AndroidWifiManager.GetScanResults();

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(counter.ToString());
            counter++;
            string wifi = "";

            for (int i = 0; i < m_scanResults.Count; i++)
            {
                var result = m_scanResults[i];
                wifi = string.Format("SSID: {0} Signal(dBm): {1} Security: {2}",
                    result.SSID,
                    result.level,
                    result.securityType);

                stringBuilder.AppendLine(wifi);
            }

            /*foreach (AndroidWifiScanResults scan in m_scanResults)
            {
                stringBuilder.Append(scan.ToString());
            }*/

            scanText.text = stringBuilder.ToString();
        }
    }
}
