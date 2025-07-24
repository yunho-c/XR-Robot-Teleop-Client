using UnityEngine;
using System.Collections.Generic;
using System;
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
    [Tooltip("The MetaSourceDataProvider component that provides the raw tracking data.")]
    // public MetaSourceDataProvider sourceDataProvider;
    public MetaSourceDataProvider sourceDataProvider;
    #endregion

    #region Public Properties
    /// <summary>
    /// Holds the most recently captured pose data.
    /// </summary>
    public PoseData CurrentPoseData { get; private set; }
    #endregion

    #region Events
    /// <summary>
    /// Event that is invoked every LateUpdate with the latest tracking data.
    /// Other scripts can subscribe to this to receive pose updates.
    /// </summary>
    public event Action<PoseData> OnPoseUpdated;
    #endregion

    #region Unity Methods
    void Start()
    {
        if (sourceDataProvider == null)
        {
            Debug.LogError("MetaSourceDataProvider not assigned to BodyPoseProvider. Disabling script.");
            this.enabled = false;
            return;
        }
        InitializePoseData();
    }

    void LateUpdate()
    {
        if (!sourceDataProvider.IsPoseValid())
        {
            return;
        }

        UpdatePoseData();
        OnPoseUpdated?.Invoke(CurrentPoseData);
    }
    #endregion

    #region Private Methods
    private void InitializePoseData()
    {
        CurrentPoseData = new PoseData();
        var tPose = sourceDataProvider.GetSkeletonTPose();
        for (int i = 0; i < tPose.Length; i++)
        {
            CurrentPoseData.bones.Add(new BoneData { id = (OVRSkeleton.BoneId)i });
        }
    }

    private void UpdatePoseData()
    {
        CurrentPoseData.timestamp = Time.time;
        var skeletonPose = sourceDataProvider.GetSkeletonPose();
        for (int i = 0; i < skeletonPose.Length; i++)
        {
            BoneData boneData = CurrentPoseData.bones[i];
            boneData.position = skeletonPose[i].Position;
            boneData.rotation = skeletonPose[i].Orientation;
            CurrentPoseData.bones[i] = boneData; // Re-assign the struct to the list
        }
    }
    #endregion
}
