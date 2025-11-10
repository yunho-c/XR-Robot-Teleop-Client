using System.Collections;
using UnityEngine;
using Unity.WebRTC;

public class MediaMTXReceiver : MonoBehaviour
{
    [SerializeField] private string urlLeft = "http://localhost:8889/stream/left/whep";
    [SerializeField] private string urlRight = "http://localhost:8889/stream/right/whep";
    [SerializeField] private Material leftMaterial;
    [SerializeField] private Material rightMaterial;

    private RTCPeerConnection pcLeft;
    private RTCPeerConnection pcRight;
    private MediaStream receiveStreamLeft;
    private MediaStream receiveStreamRight;

    void Start()
    {
        // Initialize left eye stream
        pcLeft = new RTCPeerConnection();
        receiveStreamLeft = new MediaStream();

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
                    leftMaterial.mainTexture = tex;
                };
            }
        };

        RTCRtpTransceiverInit initLeft = new RTCRtpTransceiverInit();
        initLeft.direction = RTCRtpTransceiverDirection.RecvOnly;
        pcLeft.AddTransceiver(TrackKind.Video, initLeft);

        // Initialize right eye stream
        pcRight = new RTCPeerConnection();
        receiveStreamRight = new MediaStream();

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
                    rightMaterial.mainTexture = tex;
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