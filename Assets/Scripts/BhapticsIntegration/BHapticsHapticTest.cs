using UnityEngine;
using Bhaptics.SDK2.Glove;

/// <summary>
/// Simple test script that sends haptic pulses to the bHaptics gloves every 5 seconds.
/// Attach this to an empty GameObject to test if the gloves are working.
/// </summary>
public class BHapticsHapticTest : MonoBehaviour
{
    [Header("Test Settings")]
    [Tooltip("Interval between haptic pulses (seconds)")]
    public float pulseInterval = 5f;
    
    [Tooltip("Haptic intensity (0-100)")]
    [Range(0f, 100f)]
    public float hapticIntensity = 50f;
    
    [Tooltip("Test all fingers sequentially or just thumb")]
    public bool testAllFingers = false;
    
    [Tooltip("Test both hands")]
    public bool testBothHands = true;
    
    [Header("Debug")]
    [Tooltip("Show debug logs")]
    public bool showDebugLogs = true;
    
    private BhapticsPhysicsGlove bHapticsGlove;
    private float lastPulseTime;
    private int currentFingerIndex = 0;
    private int currentHandIndex = 0; // 0 = left, 1 = right
    
    private readonly string[] fingerNames = new string[]
    {
        "Thumb", "Index", "Middle", "Ring", "Pinky"
    };
    
    void Start()
    {
        // Get bHaptics glove instance
        bHapticsGlove = BhapticsPhysicsGlove.Instance;
        
        if (bHapticsGlove == null)
        {
            Debug.LogError("[BHapticsHapticTest] No BhapticsPhysicsGlove instance found! Make sure the glove is initialized.");
        }
        else if (showDebugLogs)
        {
            Debug.Log($"[BHapticsHapticTest] Initialized. Will send haptic pulses every {pulseInterval} seconds.");
        }
        
        lastPulseTime = Time.time;
    }
    
    void Update()
    {
        if (bHapticsGlove == null)
            return;
        
        // Check if it's time for the next pulse
        if (Time.time - lastPulseTime >= pulseInterval)
        {
            SendTestHaptic();
            lastPulseTime = Time.time;
        }
    }
    
    void SendTestHaptic()
    {
        bool isLeftHand = testBothHands ? (currentHandIndex == 0) : true;
        int fingerIndex = testAllFingers ? currentFingerIndex : 0;
        
        // Send haptic pulse
        bHapticsGlove.SendEnterHaptic(isLeftHand, fingerIndex);
        
        if (showDebugLogs)
        {
            string handName = isLeftHand ? "Left" : "Right";
            string fingerName = fingerNames[fingerIndex];
            Debug.Log($"[BHapticsHapticTest] Sent haptic pulse to {handName} hand, {fingerName} finger (intensity: {hapticIntensity})");
        }
        
        // Advance to next finger/hand if testing all
        if (testAllFingers)
        {
            currentFingerIndex++;
            if (currentFingerIndex >= 5)
            {
                currentFingerIndex = 0;
                if (testBothHands)
                {
                    currentHandIndex = (currentHandIndex + 1) % 2;
                }
            }
        }
        else if (testBothHands)
        {
            currentHandIndex = (currentHandIndex + 1) % 2;
        }
    }
}

