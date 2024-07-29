using UnityEditor;
using UnityEngine;

namespace Kadinche.Kassets.EventSystem
{
    [CustomEditor(typeof(GameEvent), true)]
    [CanEditMultipleObjects]
    public class GameEventEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            AddCustomButtons();
        }

        protected virtual void AddCustomButtons()
        {
            GUI.enabled = Application.isPlaying;

            if (target is not GameEvent gameEvent)
            {
                return;
            }
            
            GUILayout.Space(15);
            
            if (!GUILayout.Button("Raise"))
            {
                return;
            }
            
            gameEvent.Raise();
            
            Debug.Log($"{target.name} event raised.");
        }
    }
    
    [CustomEditor(typeof(GameEvent<>), true)]
    [CanEditMultipleObjects]
    public class TypedGameEventEditor : GameEventEditor
    {
        protected virtual string[] ExcludedProperties { get; } = { "m_Script", "value" };

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawCustomProperties();
            
            AddCustomButtons();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawCustomProperties()
        {
            using var value = serializedObject.FindProperty("value");
            if (value.propertyType == SerializedPropertyType.Generic && !value.isArray)
            {
                foreach (var child in value.GetChildren())
                {
                    EditorGUILayout.PropertyField(child, true);
                }
            }
            else
            {
                EditorGUILayout.PropertyField(value, true);
            }

            DrawPropertiesExcluding(serializedObject, ExcludedProperties);
        }
    }
}
