using UnityEngine;
using SG;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper script to validate and assist with SenseGlove integration setup.
/// Attach this to your hand GameObject to check if everything is configured correctly.
/// </summary>
[ExecuteInEditMode]
public class SenseGloveSetupHelper : MonoBehaviour
{
    [Header("Validation")]
    [Tooltip("Enable to see validation results in Inspector")]
    public bool showValidation = true;

    [Header("Auto-Setup (Editor Only)")]
    [Tooltip("Attempt to automatically find and assign components")]
    public bool autoAssignComponents = false;

    [Header("Hand Configuration")]
    public bool isRightHand = false;

    [Header("Validation Results (Read-Only)")]
    [TextArea(10, 20)]
    public string validationResults = "Click 'Validate Setup' button to check configuration";

    private void OnValidate()
    {
        if (showValidation && Application.isPlaying == false)
        {
            ValidateSetup();
        }
    }

    public void ValidateSetup()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("=== SENSEGLOVE INTEGRATION VALIDATION ===\n");

        bool allGood = true;

        // Check for required components
        var sgTrackedHand = GetComponent<SG_TrackedHand>();
        var sgHapticGlove = GetComponent<SG_HapticGlove>();
        var adapter = GetComponent<SenseGloveToOVRSkeletonAdapter>();
        var hapticFeedback = GetComponent<SenseGloveUIHapticFeedback>();
        var ovrSkeleton = GetComponent<OVRSkeleton>();

        // 1. Check SG_TrackedHand
        if (sgTrackedHand == null)
        {
            sb.AppendLine("❌ MISSING: SG_TrackedHand component");
            sb.AppendLine("   Add SG_TrackedHand from SenseGlove SDK\n");
            allGood = false;
        }
        else
        {
            sb.AppendLine("✅ SG_TrackedHand found");
        }

        // 2. Check SG_HapticGlove
        if (sgHapticGlove == null)
        {
            sb.AppendLine("❌ MISSING: SG_HapticGlove component");
            sb.AppendLine("   Add SG_HapticGlove from SenseGlove SDK\n");
            allGood = false;
        }
        else
        {
            sb.AppendLine("✅ SG_HapticGlove found");
        }

        // 3. Check SenseGloveToOVRSkeletonAdapter
        if (adapter == null)
        {
            sb.AppendLine("❌ MISSING: SenseGloveToOVRSkeletonAdapter component");
            sb.AppendLine("   Add SenseGloveToOVRSkeletonAdapter from integration scripts\n");
            allGood = false;
        }
        else
        {
            sb.AppendLine("✅ SenseGloveToOVRSkeletonAdapter found");
            
            // Validate adapter configuration
            if (adapter.senseGloveHand == null)
            {
                sb.AppendLine("   ⚠️  WARNING: senseGloveHand not assigned");
                sb.AppendLine("      Assign the SG_TrackedHand component\n");
                allGood = false;
            }
            else
            {
                sb.AppendLine("   ✅ senseGloveHand assigned");
            }

            if (adapter.wristTrackingSource == null)
            {
                sb.AppendLine("   ⚠️  WARNING: wristTrackingSource not assigned");
                sb.AppendLine("      Assign Quest controller or camera tracker\n");
                allGood = false;
            }
            else
            {
                sb.AppendLine("   ✅ wristTrackingSource assigned");
            }

            if (adapter.isRightHand != isRightHand)
            {
                sb.AppendLine($"   ⚠️  WARNING: isRightHand mismatch");
                sb.AppendLine($"      Adapter says: {adapter.isRightHand}, Helper says: {isRightHand}\n");
            }
        }

        // 4. Check SenseGloveUIHapticFeedback
        if (hapticFeedback == null)
        {
            sb.AppendLine("❌ MISSING: SenseGloveUIHapticFeedback component");
            sb.AppendLine("   Add SenseGloveUIHapticFeedback from integration scripts\n");
            allGood = false;
        }
        else
        {
            sb.AppendLine("✅ SenseGloveUIHapticFeedback found");
            
            // Validate haptic feedback configuration
            if (hapticFeedback.hapticGlove == null)
            {
                sb.AppendLine("   ⚠️  WARNING: hapticGlove not assigned");
                sb.AppendLine("      Assign the SG_HapticGlove component\n");
                allGood = false;
            }
            else
            {
                sb.AppendLine("   ✅ hapticGlove assigned");
            }

            if (hapticFeedback.pokeInteractor == null)
            {
                sb.AppendLine("   ⚠️  WARNING: pokeInteractor not assigned");
                sb.AppendLine("      Assign PokeInteractor for button interactions\n");
            }
            else
            {
                sb.AppendLine("   ✅ pokeInteractor assigned");
            }

            if (hapticFeedback.rayInteractor == null)
            {
                sb.AppendLine("   ℹ️  INFO: rayInteractor not assigned (optional)");
            }
            else
            {
                sb.AppendLine("   ✅ rayInteractor assigned");
            }

            if (hapticFeedback.grabInteractor == null)
            {
                sb.AppendLine("   ℹ️  INFO: grabInteractor not assigned (optional)");
            }
            else
            {
                sb.AppendLine("   ✅ grabInteractor assigned");
            }
        }

        // 5. Check OVRSkeleton
        if (ovrSkeleton == null)
        {
            sb.AppendLine("❌ MISSING: OVRSkeleton component");
            sb.AppendLine("   Add OVRSkeleton from Meta SDK\n");
            allGood = false;
        }
        else
        {
            sb.AppendLine("✅ OVRSkeleton found");
            
            // Check if data provider is set (requires reflection or debug mode)
            sb.AppendLine("   ⚠️  MANUAL CHECK REQUIRED:");
            sb.AppendLine("      1. Switch Inspector to Debug mode (☰ menu → Debug)");
            sb.AppendLine("      2. Check that '_dataProvider' = SenseGloveToOVRSkeletonAdapter");
            sb.AppendLine("      3. Switch back to Normal mode\n");
        }

        // Final summary
        sb.AppendLine("========================================");
        if (allGood)
        {
            sb.AppendLine("✅ SETUP LOOKS GOOD!");
            sb.AppendLine("\nNext steps:");
            sb.AppendLine("1. Verify OVRSkeleton._dataProvider in Debug mode");
            sb.AppendLine("2. Build to Quest and test with SenseGlove gloves");
            sb.AppendLine("3. Enable debugLogging to monitor runtime behavior");
        }
        else
        {
            sb.AppendLine("❌ SETUP INCOMPLETE - Fix issues above");
            sb.AppendLine("\nSee QUICK_START.md for setup instructions");
        }

        validationResults = sb.ToString();
        Debug.Log(validationResults);
    }

#if UNITY_EDITOR
    /// <summary>
    /// Attempt to auto-assign component references in the editor.
    /// </summary>
    public void TryAutoAssignComponents()
    {
        var adapter = GetComponent<SenseGloveToOVRSkeletonAdapter>();
        var hapticFeedback = GetComponent<SenseGloveUIHapticFeedback>();
        var sgTrackedHand = GetComponent<SG_TrackedHand>();
        var sgHapticGlove = GetComponent<SG_HapticGlove>();

        bool madeChanges = false;

        // Auto-assign adapter references
        if (adapter != null)
        {
            if (adapter.senseGloveHand == null && sgTrackedHand != null)
            {
                adapter.senseGloveHand = sgTrackedHand;
                Debug.Log("Auto-assigned: adapter.senseGloveHand");
                madeChanges = true;
            }

            if (adapter.isRightHand != isRightHand)
            {
                adapter.isRightHand = isRightHand;
                Debug.Log($"Auto-set: adapter.isRightHand = {isRightHand}");
                madeChanges = true;
            }
        }

        // Auto-assign haptic feedback references
        if (hapticFeedback != null)
        {
            if (hapticFeedback.hapticGlove == null && sgHapticGlove != null)
            {
                hapticFeedback.hapticGlove = sgHapticGlove;
                Debug.Log("Auto-assigned: hapticFeedback.hapticGlove");
                madeChanges = true;
            }

            // Try to find interactors on this GameObject or children
            if (hapticFeedback.pokeInteractor == null)
            {
                var pokeInteractor = GetComponentInChildren<Oculus.Interaction.PokeInteractor>();
                if (pokeInteractor != null)
                {
                    hapticFeedback.pokeInteractor = pokeInteractor;
                    Debug.Log("Auto-assigned: hapticFeedback.pokeInteractor");
                    madeChanges = true;
                }
            }

            if (hapticFeedback.rayInteractor == null)
            {
                var rayInteractor = GetComponentInChildren<Oculus.Interaction.RayInteractor>();
                if (rayInteractor != null)
                {
                    hapticFeedback.rayInteractor = rayInteractor;
                    Debug.Log("Auto-assigned: hapticFeedback.rayInteractor");
                    madeChanges = true;
                }
            }

            if (hapticFeedback.grabInteractor == null)
            {
                var grabInteractor = GetComponentInChildren<Oculus.Interaction.GrabInteractor>();
                if (grabInteractor != null)
                {
                    hapticFeedback.grabInteractor = grabInteractor;
                    Debug.Log("Auto-assigned: hapticFeedback.grabInteractor");
                    madeChanges = true;
                }
            }
        }

        if (madeChanges)
        {
            EditorUtility.SetDirty(this);
            Debug.Log("Auto-assignment complete! Re-run validation.");
        }
        else
        {
            Debug.Log("No auto-assignments needed or possible.");
        }

        ValidateSetup();
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(SenseGloveSetupHelper))]
public class SenseGloveSetupHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SenseGloveSetupHelper helper = (SenseGloveSetupHelper)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Setup Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Validate Setup", GUILayout.Height(30)))
        {
            helper.ValidateSetup();
        }

        if (GUILayout.Button("Try Auto-Assign Components", GUILayout.Height(30)))
        {
            helper.TryAutoAssignComponents();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("1. Click 'Try Auto-Assign' to find components\n2. Click 'Validate Setup' to check configuration\n3. See validation results above", MessageType.Info);
    }
}
#endif

