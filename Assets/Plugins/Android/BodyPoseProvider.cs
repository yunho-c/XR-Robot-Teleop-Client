using UnityEngine;
using System.Collections.Generic;
using System;

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
    [Tooltip("The OVRSkeleton component that provides the raw tracking data.")]
    public OVRSkeleton skeleton;
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
        if (skeleton == null)
        {
            Debug.LogError("OVRSkeleton not assigned to BodyPoseProvider. Disabling script.");
            this.enabled = false;
            return;
        }
        InitializePoseData();
    }
    
    void LateUpdate()
    {
        if (!skeleton.IsInitialized || skeleton.Bones == null || skeleton.Bones.Count == 0 || skeleton.Bones.Count != CurrentPoseData.bones.Count)
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
        // Pre-populate the list to the exact size needed.
        for (int i = 0; i < skeleton.Bones.Count; i++)
        {
            // We only need to set the Id once. Position and rotation will be updated each frame.
            CurrentPoseData.bones.Add(new BoneData { id = skeleton.Bones[i].Id });
        }
    }
    private void UpdatePoseData()
    {
        CurrentPoseData.timestamp = Time.time;
        for (int i = 0; i < skeleton.Bones.Count; i++)
        {
            BoneData boneData = CurrentPoseData.bones[i];
            boneData.position = skeleton.Bones[i].Transform.position;
            boneData.rotation = skeleton.Bones[i].Transform.rotation;
            CurrentPoseData.bones[i] = boneData; // Re-assign the struct to the list
        }
    }
    #endregion
}
