using UnityEngine;
using SG;

/// <summary>
/// Bridges SenseGlove hand tracking data to Meta's OVRSkeleton system.
/// This allows SenseGlove-tracked hands to work with Meta Interaction SDK for UI interaction.
/// </summary>
public class SenseGloveToOVRSkeletonAdapter : MonoBehaviour, OVRSkeleton.IOVRSkeletonDataProvider
{
    [Header("SenseGlove Configuration")]
    [Tooltip("The SenseGlove TrackedHand component providing hand tracking data")]
    public SG_TrackedHand senseGloveHand;

    [Tooltip("Is this a right hand? (false for left hand)")]
    public bool isRightHand = true;

    [Header("Tracking Settings")]
    [Tooltip("External transform for wrist position (e.g., Quest controller). If null, uses local transform.")]
    public Transform wristTrackingSource;

    [Tooltip("Offset to apply to wrist position")]
    public Vector3 wristPositionOffset = Vector3.zero;

    [Tooltip("Offset to apply to wrist rotation")]
    public Vector3 wristRotationOffset = Vector3.zero;

    [Header("Debug")]
    public bool debugLogging = false;

    // Cache for bone data
    private OVRPlugin.Quatf[] _boneRotations;
    private OVRPlugin.Vector3f[] _boneTranslations;
    private bool _isInitialized = false;
    private int _skeletonChangedCount = 0;
    private SG_HandPose _lastHandPose;

    // IOVRSkeletonDataProvider interface
    public new bool enabled => this.isActiveAndEnabled && senseGloveHand != null;

    void Awake()
    {
        // Initialize bone arrays for hand skeleton (24 bones for OVR hand)
        int boneCount = (int)OVRSkeleton.BoneId.Hand_End;
        _boneRotations = new OVRPlugin.Quatf[boneCount];
        _boneTranslations = new OVRPlugin.Vector3f[boneCount];
    }

    void Start()
    {
        if (senseGloveHand == null)
        {
            Debug.LogError($"[{name}] SenseGloveToOVRSkeletonAdapter requires a SG_TrackedHand reference!");
            return;
        }

        // If no external wrist tracking, use the SenseGlove hand's transform
        if (wristTrackingSource == null)
        {
            wristTrackingSource = senseGloveHand.transform;
        }

        _isInitialized = true;
        
        if (debugLogging)
        {
            Debug.Log($"[{name}] SenseGlove adapter initialized for {(isRightHand ? "RIGHT" : "LEFT")} hand");
        }
    }

    public OVRSkeleton.SkeletonType GetSkeletonType()
    {
        return isRightHand ? OVRSkeleton.SkeletonType.HandRight : OVRSkeleton.SkeletonType.HandLeft;
    }

    public OVRSkeleton.SkeletonPoseData GetSkeletonPoseData()
    {
        var poseData = new OVRSkeleton.SkeletonPoseData
        {
            IsDataValid = false,
            IsDataHighConfidence = false,
            SkeletonChangedCount = _skeletonChangedCount
        };

        if (!_isInitialized || senseGloveHand == null)
        {
            return poseData;
        }

        // Try to get the latest hand pose from SenseGlove
        if (!senseGloveHand.GetHandPose(out SG_HandPose handPose))
        {
            if (debugLogging)
            {
                Debug.LogWarning($"[{name}] Failed to get hand pose from SenseGlove");
            }
            return poseData;
        }

        _lastHandPose = handPose;

        // Set root pose (wrist)
        poseData.RootPose = GetRootPose();
        poseData.RootScale = 1.0f;

        // Convert SenseGlove hand pose to OVR bone data
        ConvertHandPoseToOVRBones(handPose);

        poseData.BoneRotations = _boneRotations;
        poseData.BoneTranslations = _boneTranslations;
        poseData.IsDataValid = true;
        poseData.IsDataHighConfidence = true;

        return poseData;
    }

    private OVRPlugin.Posef GetRootPose()
    {
        // Get wrist pose from tracking source (controller, camera tracking, etc.)
        Vector3 position = wristTrackingSource.position + wristTrackingSource.TransformDirection(wristPositionOffset);
        Quaternion rotation = wristTrackingSource.rotation * Quaternion.Euler(wristRotationOffset);

        // Convert Unity to OVR coordinate system (flip Z)
        return new OVRPlugin.Posef
        {
            Position = position.ToFlippedZVector3f(),
            Orientation = rotation.ToFlippedZQuatf()
        };
    }

    private void ConvertHandPoseToOVRBones(SG_HandPose handPose)
    {
        // Map SenseGlove hand pose to OVR skeleton bones
        // OVR Hand skeleton has 24 bones, SenseGlove provides joint positions/rotations

        // Wrist root (Hand_Start = Hand_WristRoot = 0)
        SetBone(OVRSkeleton.BoneId.Hand_WristRoot, 
                handPose.wristPosition, 
                handPose.wristRotation);

        // Forearm stub (Hand_ForearmStub = 1)
        SetBone(OVRSkeleton.BoneId.Hand_ForearmStub, 
                Vector3.back * 0.1f, // Approximate forearm stub offset
                Quaternion.identity);

        // Thumb (4 bones: metacarpal + 3 phalanges)
        SetFingerBones(OVRSkeleton.BoneId.Hand_Thumb0, handPose, SGCore.Finger.Thumb);

        // Index finger (3 bones: proximal, middle, distal)
        SetFingerBones(OVRSkeleton.BoneId.Hand_Index1, handPose, SGCore.Finger.Index);

        // Middle finger
        SetFingerBones(OVRSkeleton.BoneId.Hand_Middle1, handPose, SGCore.Finger.Middle);

        // Ring finger
        SetFingerBones(OVRSkeleton.BoneId.Hand_Ring1, handPose, SGCore.Finger.Ring);

        // Pinky (4 bones: metacarpal + 3 phalanges)
        SetFingerBones(OVRSkeleton.BoneId.Hand_Pinky0, handPose, SGCore.Finger.Pinky);

        // Finger tips (MaxSkinnable to End: 19-24)
        SetFingerTip(OVRSkeleton.BoneId.Hand_ThumbTip, handPose, SGCore.Finger.Thumb);
        SetFingerTip(OVRSkeleton.BoneId.Hand_IndexTip, handPose, SGCore.Finger.Index);
        SetFingerTip(OVRSkeleton.BoneId.Hand_MiddleTip, handPose, SGCore.Finger.Middle);
        SetFingerTip(OVRSkeleton.BoneId.Hand_RingTip, handPose, SGCore.Finger.Ring);
        SetFingerTip(OVRSkeleton.BoneId.Hand_PinkyTip, handPose, SGCore.Finger.Pinky);
    }

    private void SetFingerBones(OVRSkeleton.BoneId startBoneId, SG_HandPose handPose, SGCore.Finger finger)
    {
        // Get joint data from SenseGlove hand pose
        Vector3[][] jointPositions = handPose.jointPositions; // [finger][joint]
        Quaternion[][] jointRotations = handPose.jointRotations;

        int fingerIndex = (int)finger;
        int boneStartIndex = (int)startBoneId;

        // Determine how many bones this finger has
        int numBones = GetFingerBoneCount(startBoneId);

        for (int i = 0; i < numBones && i < jointPositions[fingerIndex].Length; i++)
        {
            Vector3 position = jointPositions[fingerIndex][i];
            Quaternion rotation = i < jointRotations[fingerIndex].Length ? 
                                  jointRotations[fingerIndex][i] : 
                                  Quaternion.identity;

            SetBone((OVRSkeleton.BoneId)(boneStartIndex + i), position, rotation);
        }
    }

    private void SetFingerTip(OVRSkeleton.BoneId tipBoneId, SG_HandPose handPose, SGCore.Finger finger)
    {
        // Get the fingertip position
        int fingerIndex = (int)finger;
        Vector3[][] jointPositions = handPose.jointPositions;

        if (fingerIndex < jointPositions.Length && jointPositions[fingerIndex].Length > 0)
        {
            // Use the last joint position plus an offset for the tip
            int lastJointIndex = jointPositions[fingerIndex].Length - 1;
            Vector3 lastJointPos = jointPositions[fingerIndex][lastJointIndex];
            
            // Approximate tip offset (adjust based on hand model)
            Vector3 tipOffset = Vector3.forward * 0.02f; // 2cm tip extension
            Vector3 tipPosition = lastJointPos + tipOffset;

            SetBone(tipBoneId, tipPosition, Quaternion.identity);
        }
    }

    private int GetFingerBoneCount(OVRSkeleton.BoneId startBoneId)
    {
        // Thumb and Pinky have 4 bones (including metacarpal), others have 3
        if (startBoneId == OVRSkeleton.BoneId.Hand_Thumb0 || 
            startBoneId == OVRSkeleton.BoneId.Hand_Pinky0)
        {
            return 4;
        }
        return 3;
    }

    private void SetBone(OVRSkeleton.BoneId boneId, Vector3 localPosition, Quaternion localRotation)
    {
        int index = (int)boneId;
        
        if (index >= 0 && index < _boneRotations.Length)
        {
            // Convert Unity coordinates to OVR format (flip X for hand-specific coordinate system)
            _boneTranslations[index] = localPosition.ToFlippedXVector3f();
            _boneRotations[index] = localRotation.ToFlippedXQuatf();
        }
    }

    // Debug visualization
    void OnDrawGizmos()
    {
        if (!debugLogging || !Application.isPlaying || _lastHandPose == null)
            return;

        Gizmos.color = isRightHand ? Color.cyan : Color.magenta;
        
        // Draw wrist position
        if (wristTrackingSource != null)
        {
            Gizmos.DrawWireSphere(wristTrackingSource.position, 0.02f);
        }
    }
}

