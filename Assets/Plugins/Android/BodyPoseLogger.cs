using UnityEngine;
using System.Text;

/// <summary>
/// Subscribes to a BodyPoseProvider and logs the pose data it receives.
/// This component should be placed on the same GameObject as the BodyPoseProvider.
/// </summary>
[RequireComponent(typeof(BodyPoseProvider))]
public class PoseLogger : MonoBehaviour
{
    #region Public Fields
    [Tooltip("If true, logs the data as a JSON string. Otherwise, logs a simple, human-readable format.")]
    public bool logAsJson = true;
    [Tooltip("Master switch to enable or disable logging from this component.")]
    public bool enableLogging = true;
    #endregion

    #region Private Fields
    private BodyPoseProvider _bodyPoseProvider;
    private readonly StringBuilder _stringBuilder = new StringBuilder(1024); // Pre-allocate for efficiency.
    #endregion

    #region Unity Methods
    /// <summary>
    /// Caches the required BodyPoseProvider component.
    /// </summary>
    void Awake()
    {
        _bodyPoseProvider = GetComponent<BodyPoseProvider>();
    }

    /// <summary>
    /// Subscribes to the pose update event when the component is enabled.
    /// </summary>
    void OnEnable()
    {
        if (_bodyPoseProvider != null)
        {
            _bodyPoseProvider.OnPoseUpdated += HandlePoseUpdated;
        }
    }

    /// <summary>
    /// Unsubscribes from the pose update event when the component is disabled to prevent memory leaks.
    /// </summary>
    void OnDisable()
    {
        if (_bodyPoseProvider != null)
        {
            _bodyPoseProvider.OnPoseUpdated -= HandlePoseUpdated;
        }
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// The core event handler that is called by BodyPoseProvider whenever a new pose is available.
    /// </summary>
    /// <param name="poseData">The new pose data to be logged.</param>
    private void HandlePoseUpdated(BodyPoseProvider.PoseData poseData)
    {
        if (!enableLogging)
        {
            return;
        }

        if (logAsJson)
        {
            LogDataAsJson(poseData);
        }
        else
        {
            LogDataAsSimpleString(poseData);
        }
    }
    #endregion

    #region Logging Methods
    /// <summary>
    /// Logs the complete pose data to the console as a JSON string.
    /// </summary>
    /// <param name="poseData">The pose data to serialize and log.</param>
    private void LogDataAsJson(BodyPoseProvider.PoseData poseData)
    {
        string json = JsonUtility.ToJson(poseData);
        Debug.Log(json);
    }

    /// <summary>
    /// Logs the pose data to the console as a formatted, multi-line string for human readability.
    /// This method reuses a StringBuilder to minimize garbage collection.
    /// </summary>
    /// <param name="poseData">The pose data to format and log.</param>
    /// <summary>
    /// Logs the pose data to the console as a formatted, multi-line string for human readability.
    /// This method reuses a StringBuilder to minimize garbage collection.
    /// </summary>
    /// <param name="poseData">The pose data to format and log.</param>
    private void LogDataAsSimpleString(BodyPoseProvider.PoseData poseData)
    {
        // Clear the pre-allocated StringBuilder to reuse its memory.
        _stringBuilder.Clear();
        _stringBuilder.AppendLine($"--- Body Pose Data Frame (Timestamp: {poseData.timestamp:F2}) ---");

        foreach (var boneData in poseData.bones)
        {
            _stringBuilder.Append("  Bone: ");
            _stringBuilder.Append(boneData.id.ToString().PadRight(25));
            _stringBuilder.Append(" | Pos: ");
            _stringBuilder.Append(boneData.position.ToString("F4"));
            _stringBuilder.Append(" | Rot: ");
            _stringBuilder.AppendLine(boneData.rotation.ToString("F4"));
        }

        Debug.Log(_stringBuilder.ToString());
    }
    #endregion
}
