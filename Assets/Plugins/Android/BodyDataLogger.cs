using UnityEngine;
using System.Collections.Generic;
using System.Text; // Keep for the simple logging example if desired

public class BodyDataLogger : MonoBehaviour
{
    // --- Data Structures for Serialization ---

    public struct BoneData
    {
        public OVRSkeleton.BoneId id;
        public Vector3 position;
        public Quaternion rotation;
    }


    public struct PoseData
    {
        public float timestamp;
        public List<BoneData> bones;
    }

    // --- Public Fields ---

    public OVRSkeleton skeleton;
    public OVRBody body;


    public bool logAsJson = true;

    // --- Private Fields ---
    private PoseData _currentPoseData;

    // --- Unity Methods ---
    void Start()
    {

        
        if (body != null)
        {
            Debug.Log("Found body!");
        }
        else
        {
            Debug.LogWarning("Could not find body in scene.");
        }

        if (skeleton == null)
        {
            Debug.LogError("OVRSkeleton not assigned to BodyDataLogger. Disabling script.");
            this.enabled = false;
            return;
        }
        _currentPoseData = new PoseData { bones = new List<BoneData>() };
    }

    void LateUpdate()
    {
        if (!skeleton.IsInitialized || skeleton.Bones == null || skeleton.Bones.Count == 0)
        {
            return;
        }

        if (logAsJson)
        {
            LogDataAsJson();
        }
        else
        {
            LogDataAsSimpleString();
        }
    }

    // --- Logging Methods ---

    /// <summary>
    /// Populates a PoseData object and logs it to the console as a JSON string.
    /// This is the recommended approach for data serialization and streaming.
    /// </summary>
    private void LogDataAsJson()
    {
        _currentPoseData.bones.Clear();
        _currentPoseData.timestamp = Time.time;

        foreach (var bone in skeleton.Bones)
        {
            _currentPoseData.bones.Add(new BoneData
            {
                id = bone.Id,
                position = bone.Transform.position,
                rotation = bone.Transform.rotation
            });
        }

        string json = JsonUtility.ToJson(_currentPoseData);
        Debug.Log(json);
    }

    /// <summary>
    /// Logs the bone data to the console as a simple, human-readable string.
    /// Good for quick debugging but not suitable for serialization.
    /// </summary>
    private void LogDataAsSimpleString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"--- Body Pose Data Frame (Timestamp: {Time.time:F2}) ---");

        foreach (var bone in skeleton.Bones)
        {
            sb.AppendLine($"  Bone: {bone.Id.ToString().PadRight(25)} | Pos: {bone.Transform.position.ToString("F4")} | Rot: {bone.Transform.rotation.ToString("F4")}");
        }

        Debug.Log(sb.ToString());
    }
}

