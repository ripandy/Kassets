using System.Linq;
using Kadinche.Kassets.EventSystem;
using UnityEditor;

namespace Kadinche.Kassets.Variable
{
    [CustomEditor(typeof(VariableCore<>), true)]
    [CanEditMultipleObjects]
    public class VariableEditor : TypedGameEventEditor
    {
        private readonly string[] _instanceSettings = { "valueEventType", "autoResetValue" };
        private readonly string _instanceSettingsLabel = "Instance Settings";
        private bool _showInstanceSettings;

        protected override string[] ExcludedProperties => base.ExcludedProperties.Concat(_instanceSettings).ToArray();

        protected override void DrawCustomProperties()
        {
            base.DrawCustomProperties();
            
            _showInstanceSettings = EditorGUILayout.Foldout(_showInstanceSettings, _instanceSettingsLabel);

            if (_showInstanceSettings)
            {
                EditorGUI.indentLevel++;
                
                foreach (var settingName in _instanceSettings)
                {
                    using var prop = serializedObject.FindProperty(settingName);
                    if (prop == null) continue;
                    EditorGUILayout.PropertyField(prop);
                }
                
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
