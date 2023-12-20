using UnityEngine;
using System.Text;

namespace FSG.Android.Wifi
{
    public static class AndroidExtensions
    {
        /// <summary>
        /// Calls Get<T>() safely on an AndroidJavaObject. This is to safeguard against an outstanding bug in Unity 2018.2.x 
        /// which crashes Unity if the Get call returns a null string
        /// See here: https://issuetracker.unity3d.com/issues/android-application-crashes-when-native-function-returns-null
        /// 
        /// Also some versions of Android don't have all the properties we are looking for
        /// </summary>
        public static T GetFieldSafe<T>(this AndroidJavaObject javaObject, string fieldName, bool isStatic = false, params object[] args)
        {
            try
            {
                if (typeof(T) == typeof(string))
                {
#if UNITY_2018_2
                    Debug.LogErrorFormat("This version of Unity is currently not supported. See {0}", 
                        "https://issuetracker.unity3d.com/issues/android-application-crashes-when-native-function-returns-null");
                    return (T)System.Convert.ChangeType("UNITY 2018.2 NOT SUPPORTED", typeof(T));
#endif
                }
                return javaObject.Get<T>(fieldName);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                Debug.LogWarningFormat("Error getting java field \"{0}\". This version of Android might not have it.", fieldName);
                return default(T);
            }
        }

        public static AndroidJavaObject ToJavaString(this string input)
        {
            if (input == null) return null;
            try
            {
                var charsetClass = new AndroidJavaClass("java.nio.charset.Charset");
                var charset = charsetClass.CallStatic<AndroidJavaObject>("defaultCharset");
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                sbyte[] sbytes = new sbyte[bytes.Length];
                System.Buffer.BlockCopy(bytes, 0, sbytes, 0, bytes.Length);
                return new AndroidJavaObject("java.lang.String", sbytes, charset);
            }
            catch (EncoderFallbackException)
            {
                return new AndroidJavaObject("java.lang.String", input);
            }
        }
    }
}