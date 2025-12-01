using UnityEngine;
using Unity.WebRTC;
using System.Collections;
using Bhaptics.SDK2.Glove;
using Bhaptics.SDK2;

/// <summary>
/// Receives haptic messages from WebRTC and sends them to bHaptics gloves.
/// Maps 0-1 values from WebRTC messages to 0-100 intensity and vibration frequency.
/// </summary>
public class WebRTCHapticReceiver : MonoBehaviour
{
    [Header("WebRTC Integration")]
    [Tooltip("Reference to WebRTCController to access data channels. Auto-detected if not assigned.")]
    public WebRTCController webRTCController;

    [Tooltip("Name of the haptics data channel (default: 'haptics')")]
    public string hapticsChannelName = "haptics";

    [Header("bHaptics Integration")]
    [Tooltip("The bHaptics Physics Glove component. Leave null to use singleton instance.")]
    public BhapticsPhysicsGlove bHapticsGlove;

    [Header("Haptic Mapping Settings")]
    [Tooltip("Minimum haptic pulse duration in milliseconds (for high frequency)")]
    [Range(10, 100)]
    public int minPulseDurationMs = 20;

    [Tooltip("Maximum haptic pulse duration in milliseconds (for low frequency)")]
    [Range(50, 500)]
    public int maxPulseDurationMs = 200;

    [Tooltip("Enable continuous haptic updates (sends haptics every frame when values change)")]
    public bool enableContinuousHaptics = true;

    [Tooltip("Minimum intensity threshold (0-1). Values below this won't trigger haptics.")]
    [Range(0f, 0.1f)]
    public float minIntensityThreshold = 0.01f;

    [Header("Timeout Settings")]
    [Tooltip("Timeout in seconds. If no haptic messages are received within this time, haptics will stop.")]
    [Range(0.1f, 10f)]
    public float messageTimeoutSeconds = 1.0f;

    [Tooltip("Enable timeout system. If disabled, haptics will continue using last received values.")]
    public bool enableTimeout = true;

    [Header("Debug")]
    [Tooltip("Show debug logs for received haptic messages")]
    public bool showDebugLogs = false;

    // Current haptic values for each hand
    private HapticData currentLeftHaptics = new HapticData();
    private HapticData currentRightHaptics = new HapticData();

    // Track last message receive time for timeout detection
    private float lastLeftHandMessageTime = 0f;
    private float lastRightHandMessageTime = 0f;

    // Coroutine for continuous haptic updates
    private Coroutine continuousHapticCoroutine;

    // Finger index mapping: thumb=0, index=1, middle=2, ring=3, little=4, palm=5 (wrist)
    private readonly int[] fingerIndices = new int[] { 0, 1, 2, 3, 4, 5 };

    // WebRTC data channel reference
    private RTCDataChannel hapticsChannel;

    [System.Serializable]
    private class HapticMessage
    {
        public string type;
        public double timestamp;
        public HapticData left;
        public HapticData right;
    }

    [System.Serializable]
    private class HapticData
    {
        public float thumb;
        public float index;
        public float middle;
        public float ring;
        public float little;
        public float palm;
    }

    void Start()
    {
        InitializeComponents();
        SetupHapticsChannel();
    }

    void InitializeComponents()
    {
        // Auto-detect WebRTCController if not assigned
        if (webRTCController == null)
        {
            webRTCController = FindObjectOfType<WebRTCController>();
            if (webRTCController == null)
            {
                Debug.LogError("[WebRTCHapticReceiver] WebRTCController not found! Please assign it in the Inspector.");
                return;
            }
        }

        // Get bHaptics glove instance if not assigned
        if (bHapticsGlove == null)
        {
            bHapticsGlove = BhapticsPhysicsGlove.Instance;
            if (bHapticsGlove == null)
            {
                BhapticsPhysicsGlove[] allGloves = FindObjectsOfType<BhapticsPhysicsGlove>();
                if (allGloves != null && allGloves.Length > 0)
                {
                    bHapticsGlove = allGloves[0];
                    if (showDebugLogs)
                    {
                        Debug.Log("[WebRTCHapticReceiver] Found BhapticsPhysicsGlove via FindObjectsOfType");
                    }
                }
            }
            else if (showDebugLogs)
            {
                Debug.Log("[WebRTCHapticReceiver] Found BhapticsPhysicsGlove singleton instance");
            }

            if (bHapticsGlove == null)
            {
                Debug.LogWarning("[WebRTCHapticReceiver] No BhapticsPhysicsGlove instance found. Haptic feedback will be disabled.");
            }
        }

        if (showDebugLogs)
        {
            Debug.Log("[WebRTCHapticReceiver] Initialized");
        }
    }

    void SetupHapticsChannel()
    {
        // Start continuous haptic coroutine if enabled
        if (enableContinuousHaptics)
        {
            continuousHapticCoroutine = StartCoroutine(ContinuousHapticUpdate());
        }
    }

    /// <summary>
    /// Called by WebRTCController when the haptics data channel is received.
    /// </summary>
    public void OnHapticsChannelReceived(RTCDataChannel channel)
    {
        if (channel.Label == hapticsChannelName)
        {
            hapticsChannel = channel;
            SetupChannelEvents(channel);
            if (showDebugLogs)
            {
                Debug.Log($"[WebRTCHapticReceiver] Haptics channel '{hapticsChannelName}' received and set up!");
            }
        }
    }


    void SetupChannelEvents(RTCDataChannel channel)
    {
        channel.OnMessage = bytes =>
        {
            try
            {
                string message = System.Text.Encoding.UTF8.GetString(bytes);
                if (showDebugLogs)
                {
                    Debug.Log($"[WebRTCHapticReceiver] Received haptic message: {message}");
                }

                // Parse JSON message
                HapticMessage hapticMsg = JsonUtility.FromJson<HapticMessage>(message);

                if (hapticMsg != null && hapticMsg.type == "haptics")
                {
                    // Update current haptic values and message receive times
                    if (hapticMsg.left != null)
                    {
                        currentLeftHaptics = hapticMsg.left;
                        lastLeftHandMessageTime = Time.time;
                    }
                    if (hapticMsg.right != null)
                    {
                        currentRightHaptics = hapticMsg.right;
                        lastRightHandMessageTime = Time.time;
                    }

                    // Send haptics immediately (if continuous mode is disabled)
                    if (!enableContinuousHaptics)
                    {
                        SendHapticsForHand(true, currentLeftHaptics);
                        SendHapticsForHand(false, currentRightHaptics);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[WebRTCHapticReceiver] Error parsing haptic message: {e.Message}");
            }
        };
    }

    /// <summary>
    /// Continuously sends haptic updates based on current values.
    /// Respects timeout settings - stops sending if no messages received within timeout period.
    /// </summary>
    IEnumerator ContinuousHapticUpdate()
    {
        while (true)
        {
            if (bHapticsGlove != null)
            {
                float currentTime = Time.time;
                
                // Check timeout for left hand
                bool leftHandActive = true;
                if (enableTimeout)
                {
                    if (lastLeftHandMessageTime > 0f)
                    {
                        float timeSinceLastMessage = currentTime - lastLeftHandMessageTime;
                        if (timeSinceLastMessage > messageTimeoutSeconds)
                        {
                            leftHandActive = false;
                            // Clear haptic values if timeout exceeded
                            if (currentLeftHaptics != null)
                            {
                                currentLeftHaptics.thumb = 0f;
                                currentLeftHaptics.index = 0f;
                                currentLeftHaptics.middle = 0f;
                                currentLeftHaptics.ring = 0f;
                                currentLeftHaptics.little = 0f;
                                currentLeftHaptics.palm = 0f;
                            }
                            if (showDebugLogs)
                            {
                                Debug.Log($"[WebRTCHapticReceiver] Left hand timeout exceeded ({timeSinceLastMessage:F2}s > {messageTimeoutSeconds}s). Stopping haptics.");
                            }
                        }
                    }
                    else
                    {
                        // No message received yet
                        leftHandActive = false;
                    }
                }
                
                // Check timeout for right hand
                bool rightHandActive = true;
                if (enableTimeout)
                {
                    if (lastRightHandMessageTime > 0f)
                    {
                        float timeSinceLastMessage = currentTime - lastRightHandMessageTime;
                        if (timeSinceLastMessage > messageTimeoutSeconds)
                        {
                            rightHandActive = false;
                            // Clear haptic values if timeout exceeded
                            if (currentRightHaptics != null)
                            {
                                currentRightHaptics.thumb = 0f;
                                currentRightHaptics.index = 0f;
                                currentRightHaptics.middle = 0f;
                                currentRightHaptics.ring = 0f;
                                currentRightHaptics.little = 0f;
                                currentRightHaptics.palm = 0f;
                            }
                            if (showDebugLogs)
                            {
                                Debug.Log($"[WebRTCHapticReceiver] Right hand timeout exceeded ({timeSinceLastMessage:F2}s > {messageTimeoutSeconds}s). Stopping haptics.");
                            }
                        }
                    }
                    else
                    {
                        // No message received yet
                        rightHandActive = false;
                    }
                }
                
                // Only send haptics if hand is active (not timed out)
                if (leftHandActive)
                {
                    SendHapticsForHand(true, currentLeftHaptics);
                }
                if (rightHandActive)
                {
                    SendHapticsForHand(false, currentRightHaptics);
                }
            }
            yield return null; // Update every frame
        }
    }

    /// <summary>
    /// Sends haptic feedback for a specific hand.
    /// Maps 0-1 values to 0-100 intensity and uses frequency to determine pulse duration.
    /// </summary>
    void SendHapticsForHand(bool isLeft, HapticData hapticData)
    {
        if (bHapticsGlove == null || hapticData == null)
            return;

        // Create motor array (6 motors: thumb, index, middle, ring, little, wrist/palm)
        int[] motors = new int[6];

        // Map finger values (0-1) to intensity (0-100)
        float[] fingerValues = new float[]
        {
            hapticData.thumb,
            hapticData.index,
            hapticData.middle,
            hapticData.ring,
            hapticData.little,
            hapticData.palm
        };

        float maxFrequency = 0f;
        bool hasActiveHaptics = false;

        // Calculate intensity for each finger and find max frequency
        for (int i = 0; i < fingerValues.Length && i < motors.Length; i++)
        {
            float value = fingerValues[i];

            // Skip if below threshold
            if (value < minIntensityThreshold)
            {
                motors[i] = 0;
                continue;
            }

            hasActiveHaptics = true;

            // Map 0-1 to 0-100 intensity (linear mapping)
            float intensity = Mathf.Clamp01(value) * 100f;
            motors[i] = Mathf.RoundToInt(intensity);

            // Track max frequency for duration calculation
            maxFrequency = Mathf.Max(maxFrequency, value);
        }

        // Only send if there are active haptics
        if (!hasActiveHaptics)
            return;

        // Map max frequency (0-1) to pulse duration (inverse: higher value = higher frequency = shorter duration)
        // Higher frequency means faster vibration = shorter pulse duration
        float normalizedFrequency = Mathf.Clamp01(maxFrequency);
        int durationMs = Mathf.RoundToInt(
            maxPulseDurationMs - (normalizedFrequency * (maxPulseDurationMs - minPulseDurationMs))
        );

        // Position: 8 = GloveL, 9 = GloveR
        int position = isLeft ? 8 : 9; // PositionType.GloveL = 8, GloveR = 9

        // Send haptic pulse for all fingers at once
        try
        {
            BhapticsLibrary.PlayMotors(position, motors, durationMs);
            
            if (showDebugLogs)
            {
                string handName = isLeft ? "Left" : "Right";
                Debug.Log($"[WebRTCHapticReceiver] Sent haptics to {handName} hand - Thumb:{motors[0]}, Index:{motors[1]}, Middle:{motors[2]}, Ring:{motors[3]}, Little:{motors[4]}, Palm:{motors[5]} (duration: {durationMs}ms)");
            }
        }
        catch (System.Exception e)
        {
            if (showDebugLogs)
            {
                Debug.LogError($"[WebRTCHapticReceiver] Error sending haptic to {(isLeft ? "left" : "right")} hand: {e.Message}");
            }
        }
    }

    void OnDestroy()
    {
        if (continuousHapticCoroutine != null)
        {
            StopCoroutine(continuousHapticCoroutine);
        }
    }
}

