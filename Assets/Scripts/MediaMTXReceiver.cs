using System.Collections;
using UnityEngine;
using Unity.WebRTC;

public class MediaMTXReceiver : MonoBehaviour
{
    [SerializeField] private string urlLeft = "http://localhost:8889/stream/left/whep";
    [SerializeField] private string urlRight = "http://localhost:8889/stream/right/whep";
    
    // Single material with both texture slots
    [SerializeField] private Material stereoMaterial;

    private RTCPeerConnection pcLeft;
    private RTCPeerConnection pcRight;
    private MediaStream receiveStreamLeft;
    private MediaStream receiveStreamRight;

    void Start()
    {
        // Configure ICE servers
        RTCConfiguration config = new RTCConfiguration
        {
            iceServers = new[]
            {
                new RTCIceServer {urls = new[] {"stun:stun.l.google.com:19302"}}
            }
        };
        
        // Initialize left eye stream
        pcLeft = new RTCPeerConnection(ref config);
        receiveStreamLeft = new MediaStream();

        pcLeft.OnIceConnectionChange = state =>
        {
            Debug.Log($"Left ICE Connection State: {state}");
        };

        pcLeft.OnConnectionStateChange = state =>
        {
            Debug.Log($"Left Connection State: {state}");
        };

        pcLeft.OnTrack = e =>
        {
            receiveStreamLeft.AddTrack(e.Track);
        };

        receiveStreamLeft.OnAddTrack = e =>
        {
            if (e.Track is VideoStreamTrack videoTrack)
            {
                videoTrack.OnVideoReceived += (tex) =>
                {
                    // Update the Left texture property
                    if (stereoMaterial != null)
                    {
                        stereoMaterial.SetTexture("_Left", tex);
                        Debug.Log($"Left video texture updated: {tex.width}x{tex.height}");
                    }
                };
            }
        };

        RTCRtpTransceiverInit initLeft = new RTCRtpTransceiverInit();
        initLeft.direction = RTCRtpTransceiverDirection.RecvOnly;
        pcLeft.AddTransceiver(TrackKind.Video, initLeft);

        // Initialize right eye stream
        pcRight = new RTCPeerConnection(ref config);
        receiveStreamRight = new MediaStream();

        pcRight.OnIceConnectionChange = state =>
        {
            Debug.Log($"Right ICE Connection State: {state}");
        };

        pcRight.OnConnectionStateChange = state =>
        {
            Debug.Log($"Right Connection State: {state}");
        };

        pcRight.OnTrack = e =>
        {
            receiveStreamRight.AddTrack(e.Track);
        };

        receiveStreamRight.OnAddTrack = e =>
        {
            if (e.Track is VideoStreamTrack videoTrack)
            {
                videoTrack.OnVideoReceived += (tex) =>
                {
                    // Update the Right texture property (assuming it's named "Right")
                    if (stereoMaterial != null)
                    {
                        stereoMaterial.SetTexture("_Right", tex);
                        Debug.Log($"Right video texture updated: {tex.width}x{tex.height}");
                    }
                };
            }
        };

        RTCRtpTransceiverInit initRight = new RTCRtpTransceiverInit();
        initRight.direction = RTCRtpTransceiverDirection.RecvOnly;
        pcRight.AddTransceiver(TrackKind.Video, initRight);

        StartCoroutine(WebRTC.Update());
        StartCoroutine(createOffer(pcLeft, urlLeft));
        StartCoroutine(createOffer(pcRight, urlRight));
    }

    private IEnumerator createOffer(RTCPeerConnection pc, string url)
    {
        var op = pc.CreateOffer();
        yield return op;
        if (op.IsError) {
            Debug.LogError($"CreateOffer() failed for {url}");
            yield break;
        }

        yield return setLocalDescription(pc, op.Desc, url);
    }

    private IEnumerator setLocalDescription(RTCPeerConnection pc, RTCSessionDescription offer, string url)
    {
        var op = pc.SetLocalDescription(ref offer);
        yield return op;
        if (op.IsError) {
            Debug.LogError($"SetLocalDescription() failed for {url}");
            yield break;
        }

        yield return postOffer(pc, offer, url);
    }

    private IEnumerator postOffer(RTCPeerConnection pc, RTCSessionDescription offer, string url)
    {
        var content = new System.Net.Http.StringContent(offer.sdp);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/sdp");
        var client = new System.Net.Http.HttpClient();

        var task = System.Threading.Tasks.Task.Run(async () => {
            var res = await client.PostAsync(new System.UriBuilder(url).Uri, content);
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadAsStringAsync();
        });
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Exception != null) {
            Debug.LogError($"PostOffer() failed for {url}: {task.Exception}");
            yield break;
        }

        yield return setRemoteDescription(pc, task.Result, url);
    }

    private IEnumerator setRemoteDescription(RTCPeerConnection pc, string answer, string url)
    {
        RTCSessionDescription desc = new RTCSessionDescription();
        desc.type = RTCSdpType.Answer;
        desc.sdp = answer;
        var op = pc.SetRemoteDescription(ref desc);
        yield return op;
        if (op.IsError) {
            Debug.LogError($"SetRemoteDescription() failed for {url}");
            yield break;
        }

        yield break;
    }

    void OnDestroy()
    {
        pcLeft?.Close();
        pcLeft?.Dispose();
        receiveStreamLeft?.Dispose();

        pcRight?.Close();
        pcRight?.Dispose();
        receiveStreamRight?.Dispose();
    }
}