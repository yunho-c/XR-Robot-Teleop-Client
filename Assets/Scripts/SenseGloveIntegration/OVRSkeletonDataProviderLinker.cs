using UnityEngine;
using System.Reflection;

/// <summary>
/// Automatically links SenseGloveToOVRSkeletonAdapter to OVRSkeleton at runtime
/// This solves the issue where _dataProvider field is not accessible in Inspector
/// </summary>
public class OVRSkeletonDataProviderLinker : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The OVRSkeleton component to link")]
        public OVRSkeleton targetSkeleton;
        
        [Tooltip("The SenseGloveToOVRSkeletonAdapter to use as data provider")]
        public SenseGloveToOVRSkeletonAdapter adapter;

        [Header("Auto-Detection")]
        [Tooltip("Automatically find components on the same GameObject")]
        public bool autoDetectComponents = true;

        [Header("Debug")]
        [Tooltip("Show detailed debug information")]
        public bool showDebugLogs = true;

        void Awake()
        {
            LinkDataProvider();
        }

        void Start()
        {
            // Try again in Start in case components weren't ready in Awake
            if (targetSkeleton == null || adapter == null)
            {
                LinkDataProvider();
            }
        }

        /// <summary>
        /// Attempts to link the adapter to the OVRSkeleton using reflection
        /// </summary>
        public void LinkDataProvider()
        {
            // Auto-detect components if enabled
            if (autoDetectComponents)
            {
                if (targetSkeleton == null)
                {
                    targetSkeleton = GetComponent<OVRSkeleton>();
                }
                
                if (adapter == null)
                {
                    adapter = GetComponent<SenseGloveToOVRSkeletonAdapter>();
                }
            }

            // Validate components
            if (targetSkeleton == null)
            {
                LogError("No OVRSkeleton component found! Please assign one or enable auto-detection.");
                return;
            }

            if (adapter == null)
            {
                LogError("No SenseGloveToOVRSkeletonAdapter component found! Please assign one or enable auto-detection.");
                return;
            }

            // Try multiple possible field names for different SDK versions
            string[] possibleFieldNames = {
                "_dataProvider",
                "_skeletonDataProvider", 
                "_dataSource",
                "dataProvider",
                "m_DataProvider",
                "_provider"
            };

            bool success = false;
            foreach (string fieldName in possibleFieldNames)
            {
                if (TrySetDataProvider(fieldName))
                {
                    LogSuccess($"Successfully linked adapter to OVRSkeleton using field: {fieldName}");
                    success = true;
                    break;
                }
            }

            if (!success)
            {
                LogError("Could not find any compatible data provider field in OVRSkeleton!");
                LogError("This might indicate an incompatible Meta SDK version or different implementation.");
                LogError("Available fields in OVRSkeleton:");
                LogAvailableFields();
            }
        }

        /// <summary>
        /// Attempts to set the data provider using the specified field name
        /// </summary>
        private bool TrySetDataProvider(string fieldName)
        {
            try
            {
                // Get the field using reflection
                FieldInfo field = typeof(OVRSkeleton).GetField(
                    fieldName, 
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
                );

                if (field != null)
                {
                    // Check if the field type is compatible
                    System.Type fieldType = field.FieldType;
                    
                    if (fieldType.IsInterface && fieldType.Name.Contains("DataProvider"))
                    {
                        // Try to set the field
                        field.SetValue(targetSkeleton, adapter);
                        return true;
                    }
                    else if (fieldType == typeof(MonoBehaviour) || fieldType == typeof(Object))
                    {
                        // Generic object field - try setting it
                        field.SetValue(targetSkeleton, adapter);
                        return true;
                    }
                }
            }
            catch (System.Exception e)
            {
                LogDebug($"Failed to set field '{fieldName}': {e.Message}");
            }

            return false;
        }

        /// <summary>
        /// Logs all available fields in OVRSkeleton for debugging
        /// </summary>
        private void LogAvailableFields()
        {
            try
            {
                FieldInfo[] fields = typeof(OVRSkeleton).GetFields(
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
                );

                foreach (FieldInfo field in fields)
                {
                    if (field.Name.ToLower().Contains("provider") || 
                        field.Name.ToLower().Contains("data") ||
                        field.Name.ToLower().Contains("source"))
                    {
                        LogDebug($"Found potential field: {field.Name} (Type: {field.FieldType.Name})");
                    }
                }
            }
            catch (System.Exception e)
            {
                LogError($"Error logging fields: {e.Message}");
            }
        }

        /// <summary>
        /// Manually trigger linking (useful for testing)
        /// </summary>
        [ContextMenu("Link Data Provider")]
        public void ManualLink()
        {
            LinkDataProvider();
        }

        /// <summary>
        /// Test if the link is working
        /// </summary>
        [ContextMenu("Test Link")]
        public void TestLink()
        {
            if (targetSkeleton == null || adapter == null)
            {
                LogError("Components not properly assigned!");
                return;
            }

            LogDebug($"OVRSkeleton enabled: {targetSkeleton.enabled}");
            LogDebug($"Adapter enabled: {adapter.enabled}");
            LogDebug($"Adapter has SenseGlove hand: {adapter.senseGloveHand != null}");
            
            // Check if skeleton is receiving data
            if (targetSkeleton.IsDataValid)
            {
                LogSuccess("OVRSkeleton reports valid data - link appears to be working!");
            }
            else
            {
                LogDebug("OVRSkeleton reports no valid data yet - this is normal during startup");
            }
        }

        #region Logging Methods

        private void LogDebug(string message)
        {
            if (showDebugLogs)
            {
                Debug.Log($"[OVRSkeletonDataProviderLinker] {message}");
            }
        }

        private void LogSuccess(string message)
        {
            Debug.Log($"<color=green>[OVRSkeletonDataProviderLinker] {message}</color>");
        }

        private void LogError(string message)
        {
            Debug.LogError($"[OVRSkeletonDataProviderLinker] {message}");
        }

        #endregion

        #region Editor Helpers

        #if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(OVRSkeletonDataProviderLinker))]
        public class OVRSkeletonDataProviderLinkerEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                OVRSkeletonDataProviderLinker linker = (OVRSkeletonDataProviderLinker)target;

                UnityEditor.EditorGUILayout.Space();
                UnityEditor.EditorGUILayout.LabelField("Actions", UnityEditor.EditorStyles.boldLabel);

                if (GUILayout.Button("Link Data Provider"))
                {
                    linker.LinkDataProvider();
                }

                if (GUILayout.Button("Test Link"))
                {
                    linker.TestLink();
                }

                UnityEditor.EditorGUILayout.Space();
                UnityEditor.EditorGUILayout.HelpBox(
                    "This script automatically links SenseGloveToOVRSkeletonAdapter to OVRSkeleton at runtime.\n" +
                    "It solves the issue where the _dataProvider field is not accessible in the Inspector.",
                    UnityEditor.MessageType.Info
                );
            }
        }
        #endif

        #endregion
    }
