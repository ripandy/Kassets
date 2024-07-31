using Kadinche.Kassets.Variable;
using UnityEngine;

namespace Kadinche.Kassets.Sample
{
    public class AddToPrefabTest : MonoBehaviour
    {
        [SerializeField] private IntVariable someIntVariable;
        [SerializeField] private StringVariable someStringVariable;
        [SerializeField] private SomeScriptableObject someScriptableObject;
        
#if UNITY_EDITOR

        // Add
        [ContextMenu("Add Int Variable")]
        private void AddIntVariable()
        {
            someIntVariable = gameObject.Add<IntVariable>();
        }

        [ContextMenu("Add Int Variable", true)]
        private bool AddIntVariableValidate()
        {
            return gameObject.AddValidate<IntVariable>();
        }

        [ContextMenu("Add String Variable")]
        private void AddStringVariable()
        {
            someStringVariable = gameObject.Add<StringVariable>();
        }

        [ContextMenu("Add String Variable", true)]
        private bool AddStringVariableValidate()
        {
            return gameObject.AddValidate<StringVariable>();
        }

        [ContextMenu("Add SomeScriptableObject")]
        private void AddSomeScriptableObject()
        {
            someScriptableObject = gameObject.Add<SomeScriptableObject>();
        }

        [ContextMenu("Add SomeScriptableObject", true)]
        private bool AddSomeScriptableObjectValidate()
        {
            return gameObject.AddValidate<SomeScriptableObject>();
        }

        // Remove
        [ContextMenu("Remove Int Variable")]
        private void RemoveIntVariable()
        {
            gameObject.Remove<IntVariable>();
            someIntVariable = null;
        }
        
        [ContextMenu("Remove Int Variable", true)]
        private bool RemoveIntVariableValidate()
        {
            return gameObject.RemoveValidate<IntVariable>();
        }

        [ContextMenu("Remove String Variable")]
        private void RemoveStringVariable()
        {
            gameObject.Remove<StringVariable>();
            someStringVariable = null;
        }
        
        [ContextMenu("Remove String Variable", true)]
        private bool RemoveStringVariableValidate()
        {
            return gameObject.RemoveValidate<StringVariable>();
        }

        [ContextMenu("Remove SomeScriptableObject")]
        private void RemoveSomeScriptableObject()
        {
            gameObject.Remove<SomeScriptableObject>();
            someScriptableObject = null;
        }
        
        [ContextMenu("Remove SomeScriptableObject", true)]
        private bool RemoveSomeScriptableObjectValidate()
        {
            return gameObject.RemoveValidate<SomeScriptableObject>();
        }
        
#endif
    }
}