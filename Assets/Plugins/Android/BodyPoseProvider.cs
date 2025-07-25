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
        if (!sourceDataProvider.IsPoseValid())
        {
            InitializePoseData();
            _isInitialized = true;
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
        var tPose = sourceDataProvider.GetSkeletonTPose();
        if (tPose.IsCreated && tPose.Length > 0)
        {
            for (int i = 0; i < tPose.Length; i++)
            {
                CurrentPoseData.bones.Add(new BoneData { id = (OVRSkeleton.BoneId)i });
            }
            Debug.Log($"BodyPoseProvider: PoseData initialized with {tPose.Length} bones.");
        }
        else
        {
            Debug.LogError("BodyPoseProvider: Failed to get a valid T-Pose during initialization.");
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
    }
    #endregion
}

