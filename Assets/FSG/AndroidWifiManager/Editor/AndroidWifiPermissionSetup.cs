using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace FSG.Android.Wifi
{
    /// <summary>
    /// Ensure's that the required permissions are in the Android manifest
    /// </summary>
    public static class AndroidWifiPermissionSetup
    {
        // strings for asset paths and permissions
        private const string s_preferencesPath = "ProjectSettings/AndroidWifiManager.asset";
        private const string s_manifestPath = "Assets/Plugins/Android/AndroidManifest.xml";
        private const string s_manifestTemplatePath = "Assets/FSG/AndroidWifiManager/Editor/AndroidManifestTemplate.xml";
        private const string s_permissionNameTemplate = "android.permission.{0}";
        // static strings for UI
        private const string s_dialogHeader = "Android Wifi Manager Setup";
        private const string s_dialogMessage = "Enable Required Android Permission?\n\n{0}\n\n{1}";
        // required permissions for the plugin and reasoning
        private static Dictionary<string, string> s_requiredPermissions = new Dictionary<string, string>()
        {
            { "CHANGE_WIFI_STATE", "Required For:\n- Enabling and disabling wifi on the device.\n- Adding and removing configurations to the device." },
            { "ACCESS_WIFI_STATE", "Required For:\n- Accessing the current wifi state or status of the device." },
            { "ACCESS_COARSE_LOCATION", "Required For:\n- Required for scanning nearby wifi networks." }
        };

        /// <summary>
        /// Manually trigger the permissions setup
        /// </summary>
        [MenuItem("Window/Android Wifi Manager/Setup Permissions...")]
        private static void ForceSetup()
        {
            AddPermissionsIfNeeded(true);
        }

        /// <summary>
        /// Automatically trigger the permissions setup
        /// </summary>
        [InitializeOnLoadMethod]
        private static void AddPermissionsIfNeeded()
        {
            AddPermissionsIfNeeded(false);
        }

        /// <summary>
        /// Adds permissions to the AndroidManifest.xml file if needed
        /// </summary>
        /// <param name="forced">Force check every permission</param>
        private static void AddPermissionsIfNeeded(bool forced)
        {
            // load preferences
            var prefs = ReadPreferences();
            bool changed = false;
            foreach (var kvp in s_requiredPermissions)
            {
                // if not force and the permission has already been configured skip
                if (!forced && prefs.ContainsKey(kvp.Key))
                    continue;
                // ask the user for input
                string msg = string.Format(s_dialogMessage, kvp.Key, kvp.Value);
                if (EditorUtility.DisplayDialog(s_dialogHeader, msg, "OK", "Skip"))
                {
                    // add the permission
                    EnablePermission(kvp.Key);
                    // add setting to preferences
                    if (prefs.ContainsKey(kvp.Key))
                        prefs[kvp.Key] = true;
                    else
                        prefs.Add(kvp.Key, true);
                }
                else
                {
                    // add setting to preferences
                    if (prefs.ContainsKey(kvp.Key))
                        prefs[kvp.Key] = false;
                    else
                        prefs.Add(kvp.Key, false);
                }
                changed = true;
            }
            // save preferences
            if (changed)
            {
                WritePreferences(prefs);
            }
        }

        /// <summary>
        /// Reads the preferences file for this setup
        /// </summary>
        private static Dictionary<string, bool> ReadPreferences()
        {
            // preferences are stored
            // key
            // value
            // repeat...
            Dictionary<string, bool> prefs = new Dictionary<string, bool>();
            if (File.Exists(s_preferencesPath))
            {
                string[] lines = File.ReadAllLines(s_preferencesPath);
                if (lines.Length % 2 != 0)
                    return prefs;
                for (int i = 0; i < lines.Length; i += 2)
                    prefs.Add(lines[i], bool.Parse(lines[i + 1]));
            }
            return prefs;
        }

        /// <summary>
        /// Writes the preferences file for this setup
        /// </summary>
        private static void WritePreferences(Dictionary<string, bool> preferences)
        {
            // preferences are stored
            // key
            // value
            // repeat...
            StringBuilder sb = new StringBuilder();
            foreach (var kvp in preferences)
            {
                sb.AppendLine(kvp.Key);
                sb.AppendLine(kvp.Value.ToString());
            }
            File.WriteAllText(s_preferencesPath, sb.ToString().Trim());
        }

        /// <summary>
        /// Enable a specific permission in the manifest file if it doesn't already exist
        /// </summary>
        private static void EnablePermission(string permission)
        {
            // check for the manifest file
            if (!File.Exists(s_manifestPath))
            {
                // create the Plugins/Android/ folder if needed
                string[] folders = s_manifestPath.Split('/');
                string currentFolder = folders[0];
                for (int i = 1; i < folders.Length - 1; i++)
                {
                    Debug.Log(currentFolder);
                    if (!AssetDatabase.IsValidFolder(currentFolder + "/" + folders[i]))
                        AssetDatabase.CreateFolder(currentFolder, folders[i]);
                    currentFolder += '/' + folders[i];
                }
                // copy the template manifest file
                if (AssetDatabase.CopyAsset(s_manifestTemplatePath, s_manifestPath))
                {
                    Debug.LogFormat("{0}: Copied template manifest file", s_dialogHeader);
                }
            }
            string targetPermissionName = string.Format(s_permissionNameTemplate, permission);
            // load the manifest file
            XDocument document = XDocument.Load(s_manifestPath);
            // find the manifest element
            XElement manifest = document.Descendants().FirstOrDefault(q => q.Name == "manifest");
            // find all the permissions
            List<XElement> permissions = document.Descendants().Where(x => x.Name == "uses-permission").ToList();
            // if the permission doesn't exist
            if (!permissions.Any(q => q.FirstAttribute.Value == targetPermissionName))
            {
                // and it's an appropriate manifest
                if (manifest != null)
                {
                    // find the android namespace
                    XNamespace ns = manifest.GetNamespaceOfPrefix("android");
                    // create the permission
                    XAttribute attribute = new XAttribute(ns + "name", targetPermissionName);
                    XElement newPermission = new XElement("uses-permission", attribute);
                    // add permission
                    manifest.Add(newPermission);
                    // save out the new manifest
                    document.Save(s_manifestPath);
                }
            }
        }
    }
}