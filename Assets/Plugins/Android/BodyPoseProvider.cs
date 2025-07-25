using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Meta.XR.Movement.Retargeting;

public class BodyPoseProvider : MonoBehaviour
{
    #region Data Structures
    /// <summary>
    /// Represents the captured state of a single bone at a specific moment.
    /// </summary>
    [Serializable]
    public struct BoneData
    {
        public OVRSkeleton.BoneId id;
        public Vector3 position;
        public Quaternion rotation;
    }

    /// <summary>
    /// Represents a complete body pose, including a timestamp and a list of all bone data.
    /// This object is reused to prevent garbage collection.
    /// </summary>
    [Serializable]
    public class PoseData
    {
        public float timestamp;
        public List<BoneData> bones = new List<BoneData>();
    }
    #endregion

    #region Public Fields
    // [Tooltip("The MetaSourceDataProvider component that provides the raw tracking data.")]
    // public MetaSourceDataProvider sourceDataProvider;
    #endregion

    #region Public Properties
    /// <summary>
    /// Holds the most recently captured pose data.
    /// </summary>
    public PoseData CurrentPoseData { get; private set; }
    #endregion

    #region Private Fields
    private ISourceDataProvider sourceDataProvider;
    private bool _isInitialized = false;
    #endregion

    #region Events
    /// <summary>
    /// Event that is invoked every Update with the latest tracking data.
    /// Other scripts can subscribe to this to receive pose updates.
    /// </summary>
    public event Action<PoseData> OnPoseUpdated;
    #endregion

    #region Unity Methods
    void Awake()
    {
        Debug.Log("BodyPoseProvider: Awake()");
        sourceDataProvider = gameObject.GetComponent<ISourceDataProvider>();
        Assert.IsNotNull(sourceDataProvider, "BodyPoseProvider: ISourceDataProvider not found on this GameObject.");
    }

    void Start()
    {
        Debug.Log("BodyPoseProvider: Start()");
        if (sourceDataProvider == null)
        {
            Debug.LogError("BodyPoseProvider: sourceDataProvider is null. Disabling script.");
            this.enabled = false;
            return;
        }
    }

    void Update()
    {
        if (!_isInitialized)
        {
            // Attempt to initialize PoseData every frame until successful
            InitializePoseData();
            if (!_isInitialized) // If still not initialized, return early
            {
                return;
            }
        }

        if (!sourceDataProvider.IsPoseValid())
        {
            Debug.LogWarning("BodyPoseProvider: Body pose is invalid. However, using it regardless.");
            // Debug.LogWarning("BodyPoseProvider: Body pose is invalid. Waiting for valid data.");
            // return;  // TEMPDEACEAC
            // NOTE (yunho-c): I'm not sure why, but MetaSourceDataProvider seems to always return false for .IsPoseValid().
            //       Maybe because of its (weird?) temporal component and how both character retargeter & BodyPoseProvider use it.
            //       Or maybe due to the way BodyPoseProvider uses a different lifecycle hook &|| doesn't perform trasnfromation computations.
            //       Either way, this seems to be the primary reason behind silent body pose tracking failure. Thus, I'm deactivating the return guard.
            //       It is possible that a modification of the `ValidBodyPoseTrackingDelay` is the intended/correct way to fix this. 
            //       I'm just not quite sure how the validitiy decision is done and whether it causes continually persisting body pose fetch failure
            //       via `GetSkeletonPose`, but it certainly seems like the most likely cause. 
        }

        UpdatePoseData();
        OnPoseUpdated?.Invoke(CurrentPoseData);
    }
    #endregion

    #region Private Methods
    private void InitializePoseData()
    {
        Debug.Log("BodyPoseProvider: Initializing PoseData");
        CurrentPoseData = new PoseData();

        // Determine the skeleton type from the source data provider
        // Assuming MetaSourceDataProvider is the concrete type
        MetaSourceDataProvider metaSource = sourceDataProvider as MetaSourceDataProvider;
        if (metaSource == null)
        {
            Debug.LogError("BodyPoseProvider: Source data provider is not MetaSourceDataProvider. Cannot determine skeleton type.");
            return;
        }

        // Use the provided skeleton type to get the correct bone range
        OVRSkeleton.BoneId startBoneId;
        OVRSkeleton.BoneId endBoneId;

        // This logic needs to be robust. We'll assume Body or FullBody for now.
        // You might need to refine this based on the actual OVRPlugin.BodyJointSet values.
        if (metaSource.ProvidedSkeletonType == OVRPlugin.BodyJointSet.FullBody)
        {
            startBoneId = OVRSkeleton.BoneId.FullBody_Start;
            endBoneId = OVRSkeleton.BoneId.FullBody_End;
            Debug.Log("BodyPoseProvider: Detected FullBody skeleton type.");
        }
        else // Default to Body (UpperBody) if not FullBody
        {
            startBoneId = OVRSkeleton.BoneId.Body_Start;
            endBoneId = OVRSkeleton.BoneId.Body_End;
            Debug.Log("BodyPoseProvider: Detected Body (UpperBody) skeleton type.");
        }

        // Populate bones list with all possible bone IDs for the detected skeleton type
        for (int i = (int)startBoneId; i < (int)endBoneId; i++)
        {
            CurrentPoseData.bones.Add(new BoneData { id = (OVRSkeleton.BoneId)i });
        }

        if (CurrentPoseData.bones.Count > 0)
        {
            _isInitialized = true;
            Debug.Log($"BodyPoseProvider: PoseData initialized with {CurrentPoseData.bones.Count} bones.");
        }
        else
        {
            Debug.LogError("BodyPoseProvider: Failed to initialize PoseData. No bones added.");
        }
    }

    private void UpdatePoseData()
    {
        CurrentPoseData.timestamp = Time.time;
        var skeletonPose = sourceDataProvider.GetSkeletonPose();
        if (skeletonPose.IsCreated && skeletonPose.Length == CurrentPoseData.bones.Count)
        {
            for (int i = 0; i < skeletonPose.Length; i++)
            {
                BoneData boneData = CurrentPoseData.bones[i];
                boneData.position = skeletonPose[i].Position;
                boneData.rotation = skeletonPose[i].Orientation;
                CurrentPoseData.bones[i] = boneData;
            }
        }
        else if (skeletonPose.IsCreated && skeletonPose.Length != CurrentPoseData.bones.Count)
        {
            Debug.LogWarning($"BodyPoseProvider: Mismatch in bone count. Expected {CurrentPoseData.bones.Count}, got {skeletonPose.Length}. Re-initializing PoseData.");
            _isInitialized = false;
        }
    }
    #endregion
}