using Bhaptics.SDK2.Glove;
using UnityEditor;
using UnityEngine;

namespace Bhaptics.SDK2
{
    [CustomEditor(typeof(BhapticsPhysicsGlove))]
    public class BhapticsPhysicsGloveEditor : UnityEditor.Editor
    {
        private const string SettingsPathBase = "Assets/Bhaptics/SDK2/Resources/GloveHapticSettings";
        private const string SettingsExtension = ".asset";

        private BhapticsPhysicsGloveSettings LoadOrCreateSettingsAsset()
        {
            string settingsPath = SettingsPathBase + SettingsExtension;

            if (System.IO.File.Exists(settingsPath))
            {
                int count = 1;
                while (true)
                {
                    settingsPath = $"{SettingsPathBase} {count}{SettingsExtension}";

                    if (!System.IO.File.Exists(settingsPath))
                    {
                        break;
                    }
                    
                    count++;
                }
            }
            var settings = ScriptableObject.CreateInstance<BhapticsPhysicsGloveSettings>();
            AssetDatabase.CreateAsset(settings, settingsPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return settings;
        }

        public override void OnInspectorGUI()
        {
            BhapticsPhysicsGlove controller = (BhapticsPhysicsGlove)target;

            serializedObject.Update();

            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Bhaptics Physics Glove Settings", EditorStyles.boldLabel);

            if (controller.HapticSettings == null)
            {
                if (GUILayout.Button(new GUIContent("Create New GloveHapticSettings", "Creates a new GloveHapticSettings asset in the path: Assets/Bhaptics/SDK2/Resources")))
                {
                    controller.HapticSettings = LoadOrCreateSettingsAsset();
                    EditorUtility.SetDirty(controller);  // Mark the controller as dirty to save the changes
                }
            }
            else
            {
                UnityEditor.Editor editor = CreateEditor(controller.HapticSettings);
                editor.OnInspectorGUI();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Save Changes"))
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Revert to Original"))
            {
                serializedObject.ApplyModifiedProperties();
                ReturnOriginalValue(controller.HapticSettings);
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            serializedObject.ApplyModifiedProperties();
        }

        public void ReturnOriginalValue(BhapticsPhysicsGloveSettings gloveSettings)
        {
            gloveSettings.motorIntensityMax = 50;
            gloveSettings.motorIntensityMin = 1;
            gloveSettings.velocityChangeMax = 2.0f;
            gloveSettings.velocityChangeMin = 0.2f;
            gloveSettings.decayRate = 0.3f;
            gloveSettings.decayDelay = 0.5f;
            gloveSettings.masterSlaveDistanceMax = 20f;
        }
    }
}