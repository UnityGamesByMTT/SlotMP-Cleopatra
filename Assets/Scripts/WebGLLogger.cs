#if UNITY_WEBGL && !UNITY_EDITOR
using UnityEngine;

public class WebGLLogger : MonoBehaviour
{
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string formattedMessage = $"[{type}] {logString.Replace("\"", "\\\"")}";

        // Optionally include stack trace
        // formattedMessage += $"\n{stackTrace.Replace("\"", "\\\"")}";

        Application.ExternalEval($@"
            if(window.ReactNativeWebView){{
                window.ReactNativeWebView.postMessage(""{formattedMessage}"");
            }}
        ");
    }
}
#endif
