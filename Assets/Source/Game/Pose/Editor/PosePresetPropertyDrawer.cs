using AudioChat.Pose;
using UnityEditor;
using UnityEngine;

namespace AudioChat.Editor
{
    [CustomPropertyDrawer(typeof(PoseContainer.PosePreset))]
    public class PosePresetPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (20f + (property.isExpanded ? 70f : 0f));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty nameProperty = property.FindPropertyRelative("Name");
            SerializedProperty poseProperty = property.FindPropertyRelative("Pose");

            Rect rect = position;
            rect.height = 20f;
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, nameProperty.stringValue);

            if (property.isExpanded)
            {
                ++EditorGUI.indentLevel;
                rect.y += 22f;
                EditorGUI.PropertyField(rect, nameProperty);
                rect.y += 22f;
                EditorGUI.PropertyField(rect, poseProperty);
                rect.y += 22f;
                if (GUI.Button(rect, "Open in editor"))
                {
                    PoseEditorWindow.Open(poseProperty);
                }
                --EditorGUI.indentLevel;
            }
        }
    }
}
