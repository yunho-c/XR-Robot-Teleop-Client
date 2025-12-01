using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Bhaptics.SDK2
{
    [CustomEditor(typeof(BhapticsPhysicsGloveSettings))]
    public class BhapticsPhysicsGloveSettingsEditor : UnityEditor.Editor
    {
        SerializedProperty hapticMode;
        SerializedProperty motorIntensityMax;
        SerializedProperty motorIntensityMin;
        SerializedProperty velocityChangeMax;
        SerializedProperty velocityChangeMin;
        SerializedProperty decayRate;
        SerializedProperty decayDelay;
        SerializedProperty masterSlaveDistanceMax;

        void OnEnable()
        {
            if (target != null)
            { 
                hapticMode = serializedObject.FindProperty("hapticMode");
                motorIntensityMax = serializedObject.FindProperty("motorIntensityMax");
                motorIntensityMin = serializedObject.FindProperty("motorIntensityMin");
                velocityChangeMax = serializedObject.FindProperty("velocityChangeMax");
                velocityChangeMin = serializedObject.FindProperty("velocityChangeMin");
                decayRate = serializedObject.FindProperty("decayRate");
                decayDelay = serializedObject.FindProperty("decayDelay");
                masterSlaveDistanceMax = serializedObject.FindProperty("masterSlaveDistanceMax");
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Glove Haptic Mode", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(hapticMode);
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            BhapticsPhysicsGloveSettings.HapticMode mode = (BhapticsPhysicsGloveSettings.HapticMode)hapticMode.enumValueIndex;

            EditorGUILayout.LabelField("Glove Haptic Parameter", EditorStyles.boldLabel);

            switch (mode)
            {
                case BhapticsPhysicsGloveSettings.HapticMode.Mode1:

                    EditorGUILayout.PropertyField(velocityChangeMax);
                    EditorGUILayout.PropertyField(velocityChangeMin);
                    EditorGUILayout.PropertyField(motorIntensityMax);
                    EditorGUILayout.PropertyField(motorIntensityMin);
                    EditorGUILayout.PropertyField(decayRate);
                    EditorGUILayout.PropertyField(decayDelay);
                    EditorGUILayout.PropertyField(masterSlaveDistanceMax);

                    break;
                case BhapticsPhysicsGloveSettings.HapticMode.Mode2:

                    EditorGUILayout.PropertyField(velocityChangeMax);
                    EditorGUILayout.PropertyField(motorIntensityMax);
                    EditorGUILayout.PropertyField(motorIntensityMin);
                    EditorGUILayout.PropertyField(decayRate);
                    EditorGUILayout.PropertyField(decayDelay);

                    break;
                case BhapticsPhysicsGloveSettings.HapticMode.Mode3:

                    EditorGUILayout.PropertyField(velocityChangeMax);
                    EditorGUILayout.PropertyField(motorIntensityMax);
                    EditorGUILayout.PropertyField(motorIntensityMin);

                    break;
                case BhapticsPhysicsGloveSettings.HapticMode.Mode4:

                    EditorGUILayout.PropertyField(velocityChangeMax);
                    EditorGUILayout.PropertyField(motorIntensityMax);
                    EditorGUILayout.PropertyField(motorIntensityMin);

                    break;
            }

            serializedObject.ApplyModifiedProperties();

        }
    }
}
