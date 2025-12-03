using UnityEditor;
using UnityEngine;

namespace Bhaptics.SDK2
{
    [CustomPropertyDrawer(typeof(BhapticsPhysicsGloveSettings.HapticMode))]
    public class BhapticsPhysicsGloveModeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            GUIContent[] enumDescriptions = new GUIContent[]
            {
                new GUIContent("Master,Slave mode"),
                new GUIContent("No Master,Slave, No relative velocity"),
                new GUIContent("No Master,Slave, No relative velocity, No decay over time"),
                new GUIContent("No Master,Slave, No relative velocity, No decay over time, No collision")
            };

            property.enumValueIndex = EditorGUI.Popup(position, label, property.enumValueIndex, enumDescriptions);

            EditorGUI.EndProperty();
        }
    }
}
