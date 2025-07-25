
using UnityEngine;
using TMPro;

public class IpAddressSync : MonoBehaviour
{
    public WebRTCController webRTCController;
    public TMP_InputField ipAddressInputField;

    void Start()
    {
        if (webRTCController != null && ipAddressInputField != null)
        {
            string serverUrl = webRTCController.serverUrl;
            if (!string.IsNullOrEmpty(serverUrl))
            {
                // Extract IP address from the server URL
                try
                {
                    System.Uri uri = new System.Uri(serverUrl);
                    ipAddressInputField.text = uri.Host;
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error parsing server URL: " + e.Message);
                }
            }
        }
    }
}
