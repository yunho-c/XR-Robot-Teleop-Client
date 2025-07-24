
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.WebRTC;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;


[System.Serializable]
public class OrientationState
{
    public float yaw;
    public float pitch;
    public float roll;
    public float fov_x = 90.0f;
}


public class WebRTCController : MonoBehaviour
{
    [Header("Signaling Server")]
    [Tooltip("URL of the signaling server")]
    public string serverUrl = "http://localhost:8080/offer";

    [Header("VR Camera")]
    [Tooltip("The VR camera to track")]
    public Camera vrCamera;

    [Header("Body Tracking")]
    // #if UNITY_ANDROID
    [Tooltip("The BodyPoseProvider to get body pose data from")]
    public BodyPoseProvider bodyPoseProvider;
    // #endif

    [Header("UI Elements")]
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private RawImage videoImage;
    [SerializeField] private TMP_InputField ipAddressInputField;

    [Header("WebRTC Settings")]
    [Tooltip("Enable to receive video stream")]
    public bool receiveVideo = true;

    private RTCPeerConnection pc;
    private RTCDataChannel cameraChannel;
    private RTCDataChannel bodyPoseChannel;

    void Start()
    {
        serverUrl = PlayerPrefs.GetString("serverUrl", serverUrl);
        statusText.text = "Ready to connect.";

        if (ipAddressInputField != null)
        {
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

    void Update()
    {
        if (cameraChannel != null && cameraChannel.ReadyState == RTCDataChannelState.Open)
        {
            SendOrientation();
        }

        // #if UNITY_ANDROID
        if (bodyPoseChannel != null && bodyPoseChannel.ReadyState == RTCDataChannelState.Open && bodyPoseProvider != null)
        {
            SendBodyPoseData();
        }
        // #endif
    }

    public void SetServerIp(string ipAddress)
    {
        serverUrl = "http://" + ipAddress + ":8080/offer";
        PlayerPrefs.SetString("serverUrl", serverUrl);
        PlayerPrefs.Save();
        statusText.text = $"Server URL set to: {serverUrl}";
        Debug.Log("Server URL set to: " + serverUrl);
    }

    public void StartConnection()
    {
        if (pc != null && (pc.ConnectionState == RTCPeerConnectionState.Connected || pc.ConnectionState == RTCPeerConnectionState.Connecting))
        {
            Debug.LogWarning("WebRTC connection is already active or connecting.");
            return;
        }
        statusText.text = "Starting WebRTC...";
        StartCoroutine(StartWebRTC());
    }

    public void StopConnection()
    {
        if (cameraChannel != null)
        {
            cameraChannel.Close();
            cameraChannel = null;
        }
        if (bodyPoseChannel != null)
        {
            bodyPoseChannel.Close();
            bodyPoseChannel = null;
        }
        if (pc != null)
        {
            pc.Close();
            pc = null;
        }
        statusText.text = "Disconnected.";
        Debug.Log("WebRTC connection closed.");
    }

    public void ToggleConnection(bool isOn)
    {
        if (isOn)
        {
            StartConnection();
        }
        else
        {
            StopConnection();
        }
    }

    // #if UNITY_ANDROID
    private void SendBodyPoseData()
    {
        if (bodyPoseProvider.CurrentPoseData.bones != null && bodyPoseProvider.CurrentPoseData.bones.Count > 0)
        {
            string jsonPose = JsonUtility.ToJson(bodyPoseProvider.CurrentPoseData);
            bodyPoseChannel.Send(jsonPose);
        }
    }
    // #endif

    private IEnumerator StartWebRTC()
    {
        CreatePeerConnection();

        // Add video transceiver if video is enabled
        if (receiveVideo)
        {
            var videoTransceiver = pc.AddTransceiver(TrackKind.Video);
            videoTransceiver.Direction = RTCRtpTransceiverDirection.RecvOnly;
        }

        // Create data channel
        cameraChannel = pc.CreateDataChannel("camera");
        SetupDataChannelEvents(cameraChannel);

        // Create pose data channel
        bodyPoseChannel = pc.CreateDataChannel("body_pose");
        SetupDataChannelEvents(bodyPoseChannel);

        // Create offer
        var offer = pc.CreateOffer();
        yield return offer;

        if (offer.IsError)
        {
            Debug.LogError("Error creating offer: " + offer.Error.message);
            yield break;
        }

        var desc = offer.Desc;
        var localDescOp = pc.SetLocalDescription(ref desc);
        yield return localDescOp;

        if (localDescOp.IsError)
        {
            Debug.LogError("Error setting local description: " + localDescOp.Error.message);
            yield break;
        }

        // Send offer to server
        statusText.text = $"Sending offer to {serverUrl}...";
        SignalingMessage offerMessage = new SignalingMessage { type = "offer", sdp = desc.sdp };
        string jsonOffer = JsonUtility.ToJson(offerMessage);

        using (UnityWebRequest www = new UnityWebRequest(serverUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonOffer);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error sending offer: " + www.error);
                statusText.text = $"Error sending offer: {www.error}";
                yield break;
            }

            statusText.text = "Offer sent, waiting for answer...";
            string jsonAnswer = www.downloadHandler.text;
            SignalingMessage answerMessage = JsonUtility.FromJson<SignalingMessage>(jsonAnswer);
            StartCoroutine(OnGotAnswer(answerMessage.sdp));
        }
    }

    private void CreatePeerConnection()
    {
        var configuration = GetSelectedSdpSemantics();
        pc = new RTCPeerConnection(ref configuration);
        Debug.Log("Peer Connection created.");

        pc.OnConnectionStateChange = state =>
        {
            Debug.Log("Connection state changed to: " + state);
            if (state == RTCPeerConnectionState.Connected)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    statusText.text = "Peers connected!";
                });
            }
        };

        pc.OnDataChannel = channel =>
        {
            Debug.Log($"Data Channel received: {channel.Label}!");
            if (channel.Label == "camera")
            {
                cameraChannel = channel;
            }
            else if (channel.Label == "body_pose")
            {
                bodyPoseChannel = channel;
            }
            SetupDataChannelEvents(channel);
        };

        // The client receives the video stream
        pc.OnTrack = (RTCTrackEvent e) =>
        {
            if (e.Track.Kind == TrackKind.Video)
            {
                if (e.Track is VideoStreamTrack videoTrack)
                {
                    videoTrack.OnVideoReceived += (texture) =>
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            if (videoImage != null)
                            {
                                videoImage.texture = texture;
                            }
                        });
                    };
                }
            }
        };
    }

    private IEnumerator OnGotAnswer(string sdp)
    {
        var remoteDesc = new RTCSessionDescription { type = RTCSdpType.Answer, sdp = sdp };
        var remoteDescOp = pc.SetRemoteDescription(ref remoteDesc);
        yield return remoteDescOp;

        if (remoteDescOp.IsError)
        {
            Debug.LogError("Error setting remote description on answer: " + remoteDescOp.Error.message);
        }
    }

    private void SetupDataChannelEvents(RTCDataChannel channel)
    {
        channel.OnOpen = () =>
        {
            Debug.Log($"{channel.Label} Channel is open!");
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                statusText.text = $"{channel.Label} channel open.";
            });
        };

        channel.OnClose = () =>
        {
            Debug.Log($"{channel.Label} Channel is closed!");
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                statusText.text = $"{channel.Label} channel closed.";
            });
        };

        channel.OnMessage = bytes =>
        {
            // Handle incoming messages if needed
            Debug.Log($"Received message on {channel.Label} channel: {System.Text.Encoding.UTF8.GetString(bytes)}");
        };
    }

    private void SendOrientation()
    {
        if (vrCamera != null)
        {
            OrientationState state = new OrientationState
            {
                yaw = vrCamera.transform.eulerAngles.y,
                pitch = -vrCamera.transform.eulerAngles.x, // Invert pitch for correct mapping
                roll = vrCamera.transform.eulerAngles.z
            };
            string jsonState = JsonUtility.ToJson(state);
            cameraChannel.Send(jsonState);
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("serverUrl", serverUrl);
        PlayerPrefs.Save();
        StopConnection();
    }

    private static RTCConfiguration GetSelectedSdpSemantics()
    {
        return new RTCConfiguration
        {
            iceServers = new[] { new RTCIceServer { urls = new[] { "stun:stun.l.google.com:19302" } } }
        };
    }
}
