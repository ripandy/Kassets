using System;
using Kadinche.Kassets.EventSystem;
using UnityEngine;

namespace Kadinche.Kassets.Variable
{
    /// <summary>
    /// Variable System Basics.
    /// </summary>
    /// <typeparam name="T">Type to use on variable system</typeparam>
    public abstract class VariableCore<T> : GameEvent<T>
    {
        [Tooltip("Set how value event behave.\nValue Assign: Raise when value is assigned regardless of value.\nValue Changed: Raise only when value is changed.")]
        [SerializeField] protected ValueEventType valueEventType;

        [Tooltip("If true will reset value when play mode end. Otherwise, keep runtime value. Due to shallow copying of class types, it is better avoid using autoResetValue on Class type.")]
        [SerializeField] protected bool autoResetValue;
        
        public virtual T Value
        {
            get => value;
            set => Raise(value);
        }

        public override void Raise(T valueToRaise)
        {
            if (valueEventType == ValueEventType.OnChange && IsValueEquals(valueToRaise)) return;
            base.Raise(valueToRaise);
        }

        private bool IsValueEquals(T valueToCompare) => value == null && valueToCompare == null || 
                                                        value != null && valueToCompare != null && value.Equals(valueToCompare);
        
        private T initialValue;
        private string initialValueJsonString; // Hack to handle shallow copy of class type. Better to avoid this on class type.

        public T InitialValue
        {
            get
            {
                if (!Type.IsSimpleType())
                {
                    try
                    {
                        initialValue = JsonUtility.FromJson<T>(initialValueJsonString);
                    }
                    catch (ArgumentException)
                    {
                        // TODO : actually check for Engine Types or "Fix" the hack.
                    }
                }
                return initialValue;
            }
            protected set
            {
                if (!Type.IsSimpleType())
                {
                    try
                    {
                        initialValueJsonString = JsonUtility.ToJson(value);
                    }
                    catch (ArgumentException)
                    {
                        // TODO : actually check for Engine Types or "Fix" the hack.
                    }
                }
                initialValue = value;
            }
        }

        /// <summary>
        /// Reset value to InitialValue
        /// </summary>
        public void ResetValue() => Value = InitialValue;

        protected override void ResetInternal()
        {
            if (!autoResetValue)
            {
                return;
            }
            
            ResetValue();
        }

        public static implicit operator T(VariableCore<T> variable) => variable.Value;

        public override string ToString() => Value.ToString();

        protected override void Initialize()
        {
            InitialValue = Value;
            base.Initialize();
        }
    }
}