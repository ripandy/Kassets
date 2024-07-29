using System;
using UnityEngine;

namespace Kadinche.Kassets.EventSystem
{
    /// <summary>
    /// Core Game Event System.
    /// </summary>
    [CreateAssetMenu(fileName = "GameEvent", menuName = MenuHelper.DefaultGameEventMenu + "GameEvent")]
    public partial class GameEvent : KassetsCore
    {
    }

    /// <summary>
    /// Generic base class for event system with parameter.
    /// </summary>
    /// <typeparam name="T">Parameter type for the event system</typeparam>
    public abstract partial class GameEvent<T> : GameEvent
    {
        [SerializeField] protected T value;

        public Type Type => typeof(T);

        public override void Raise() => Raise(value);

        protected virtual void ResetInternal()
        {
            value = default;
        }

        protected override void OnQuit()
        {
            ResetInternal();
            base.OnQuit();
        }
    }
}
