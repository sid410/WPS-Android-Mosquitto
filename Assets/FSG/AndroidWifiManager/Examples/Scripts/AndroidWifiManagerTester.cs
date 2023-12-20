using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSG.Android.Wifi
{
    public class AndroidWifiManagerTester : MonoBehaviour
    {
        private const float s_displaySize = 0.95f;

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

        /// <summary>
        /// Pull initial values
        /// </summary>
        private void Awake()
        {
            m_currentState = AndroidWifiManager.GetWifiState();
            m_configuredNetworks = AndroidWifiManager.GetConfiguredNetworks();
            m_scanResults = AndroidWifiManager.GetScanResults();
        }

        /// <summary>
        /// Update touch input for scroll view
        /// </summary>
        private void Update()
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);
                switch (t.phase)
                {
                    case TouchPhase.Moved:
                        m_scrollPos.y += t.deltaPosition.y;
                        break;
                }
            }
        }

        /// <summary>
        /// Render debug items
        /// </summary>
        private void OnGUI()
        {
            if (m_buttonStyle == null)
            {
                m_buttonStyle = new GUIStyle(GUI.skin.button);
                m_buttonStyle.fontSize = 50;
                m_buttonListStyle = new GUIStyle(GUI.skin.button);
                m_buttonListStyle.fontSize = 25;
                m_textStyle = new GUIStyle(GUI.skin.label);
                m_textStyle.fontSize = 50;
                m_listStyle = new GUIStyle(GUI.skin.label);
                m_listStyle.fontSize = 25;
                m_toggleStyle = new GUIStyle(GUI.skin.toggle);
                m_toggleStyle.fontSize = 25;
                m_passwordStyle = new GUIStyle(GUI.skin.textField);
                m_passwordStyle.fontSize = 50;
            }
            int w = Screen.width;
            int h = Screen.height;
            GUILayout.BeginArea(new Rect(w * 0.5f - w * s_displaySize * 0.5f, h * 0.5f - h * s_displaySize * 0.5f, w * s_displaySize, h * s_displaySize));
            {
                m_scrollPos = GUILayout.BeginScrollView(m_scrollPos, GUI.skin.box);
                GUILayout.BeginVertical();
                {
                    if (m_enteringPassword)
                    {
                        OnGUI_EnterPassword();
                    }
                    else
                    {
                        OnGUI_WifiStatus();
                        OnGUI_Scanning();
                        OnGUI_Configurations();
                    }
                }
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();
        }
        private void OnGUI_EnterPassword()
        {
            if (m_connectingNetwork == null)
            {
                m_enteringPassword = false;
                return;
            }
            if (m_connectingNetwork.securityType == AndroidWifiSecurityType.OPEN)
            {
                Debug.Log("Auto connecting because the network has no security");
                var result = AndroidWifiManager.Connect(m_connectingNetwork.SSID);
                Debug.LogFormat("Connect Result: {0}", result);
                m_enteringPassword = false;
                m_connectingNetwork = null;
                return;
            }
            GUILayout.Label(string.Format("Network {0}", m_connectingNetwork.SSID), m_textStyle);
            GUILayout.Label("Enter Password:", m_textStyle);
            m_password = GUILayout.TextField(m_password, m_passwordStyle);
            if (GUILayout.Button("Connect", m_buttonStyle))
            {
                var result = AndroidWifiManager.Connect(m_connectingNetwork.SSID, m_password);
                Debug.LogFormat("Connect Result: {0}", result);
                if (result == AndroidWifiManager.ConnectResult.SUCCESS)
                {
                    m_enteringPassword = false;
                    m_connectingNetwork = null;
                }
            }
            GUILayout.Space(100);
            if (GUILayout.Button("Cancel", m_buttonStyle))
            {
                m_enteringPassword = false;
                m_connectingNetwork = null;
            }

        }
        private void OnGUI_WifiStatus()
        {
            var current = m_configuredNetworks.Find(q => q.status == AndroidWifiConfiguration.Status.CURRENT);
            GUILayout.Label(string.Format("Current Network: {0}", current != null ? current.SSID : "UNKNOWN"), m_textStyle);
            GUILayout.Space(10);
            if (GUILayout.Button("SetWifiEnabled(true)", m_buttonStyle))
            {
                Debug.LogFormat("Set Wifi Enabled Result: {0}", AndroidWifiManager.SetWifiEnabled(true));
            }
            GUILayout.Space(10);
            if (GUILayout.Button("SetWifiEnabled(false)", m_buttonStyle))
            {
                Debug.LogFormat("Set Wifi Disabled Result: {0}", AndroidWifiManager.SetWifiEnabled(false));
            }
            GUILayout.Label(string.Format("Wifi Enabled: {0}", AndroidWifiManager.IsWifiEnabled()), m_textStyle);
            GUILayout.Space(10);
            GUILayout.Label(string.Format("Wifi State: {0}", m_currentState), m_textStyle);
            if (GUILayout.Button("GetWifiState()", m_buttonStyle))
            {
                m_currentState = AndroidWifiManager.GetWifiState();
            }
            GUILayout.Space(10);
        }
        private void OnGUI_Scanning()
        {
            if (GUILayout.Button("StartScan()", m_buttonStyle))
            {
                Debug.LogFormat("Starting Scan: {0}", AndroidWifiManager.StartScan());
            }
            GUILayout.Space(10);
            if (GUILayout.Button("GetScanResults()", m_buttonStyle))
            {
                m_scanResults = AndroidWifiManager.GetScanResults();
            }
            GUILayout.Label(string.Format("Scan Results: {0}", m_scanResults.Count), m_textStyle);
            for (int i = 0; i < m_scanResults.Count; i++)
            {
                var result = m_scanResults[i];
                GUILayout.Label(string.Format("SSID: {0} Signal(dBm): {1} Security: {2}",
                    result.SSID,
                    result.level,
                    result.securityType),
                    m_listStyle);
                if (GUILayout.Button("Connect", m_buttonListStyle))
                {
                    m_password = string.Empty;
                    m_connectingNetwork = result;
                    m_enteringPassword = true;
                }
            }
            GUILayout.Space(10);
        }
        private void OnGUI_Configurations()
        {
            if (GUILayout.Button("GetConfiguredNetworks()", m_buttonStyle))
            {
                m_configuredNetworks = AndroidWifiManager.GetConfiguredNetworks();
            }
            GUILayout.Label(string.Format("Configured Networks: {0}", m_configuredNetworks.Count), m_textStyle);
            m_showOnlyCurrent = GUILayout.Toggle(m_showOnlyCurrent, "Show Only Current Network", m_toggleStyle);
            for (int i = 0; i < m_configuredNetworks.Count; i++)
            {
                var config = m_configuredNetworks[i];
                if (m_showOnlyCurrent && config.status != AndroidWifiConfiguration.Status.CURRENT)
                    continue;
                GUILayout.Label(string.Format("Status: {0} SSID: {1} Network Id: {2}", config.status, config.SSID, config.networkId), m_listStyle);
                GUILayout.BeginHorizontal();
                {
                    switch (config.status)
                    {
                        case AndroidWifiConfiguration.Status.CURRENT:
                            {
                                if (GUILayout.Button("Disconnect", m_buttonListStyle))
                                {
                                    var callResult = AndroidWifiManager.Disconnect();
                                    Debug.LogFormat("Disconnect Result: {0}", callResult);
                                    m_configuredNetworks = AndroidWifiManager.GetConfiguredNetworks();
                                }
                                break;
                            }
                        case AndroidWifiConfiguration.Status.DISABLED:
                            {
                                if (GUILayout.Button("Enable", m_buttonListStyle))
                                {
                                    var callResult = AndroidWifiManager.EnableNetwork(config.networkId, false);
                                    Debug.LogFormat("Enable Network Result: {0}", callResult);
                                    m_configuredNetworks = AndroidWifiManager.GetConfiguredNetworks();
                                }
                                break;
                            }
                        case AndroidWifiConfiguration.Status.ENABLED:
                            {
                                if (GUILayout.Button("Disable", m_buttonListStyle))
                                {
                                    var callResult = AndroidWifiManager.DisableNetwork(config.networkId);
                                    Debug.LogFormat("Disable Network Result: {0}", callResult);
                                    m_configuredNetworks = AndroidWifiManager.GetConfiguredNetworks();
                                }
                                break;
                            }
                    }
                    if (GUILayout.Button("Forget", m_buttonListStyle))
                    {
                        var callResult = AndroidWifiManager.RemoveNetwork(config.networkId);
                        Debug.LogFormat("Forget Result: {0}", callResult);
                        m_configuredNetworks = AndroidWifiManager.GetConfiguredNetworks();
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(100);
            }
        }
    }
}