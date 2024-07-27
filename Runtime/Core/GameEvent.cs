#if !KASSETS_R3
using System.Collections.Generic;
#endif

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
        [SerializeField] protected T _value;

        public Type Type => typeof(T);

        public override void Raise() => Raise(_value);

        protected virtual void ResetInternal()
        {
            _value = default;
        }

        protected override void OnQuit()
        {
            ResetInternal();
            base.OnQuit();
        }
    }

#if !KASSETS_R3
    public partial class GameEvent
    {
        protected readonly IList<IDisposable> disposables = new List<IDisposable>();

        /// <summary>
        /// Raise the event.
        /// </summary>
        public virtual void Raise()
        {
            foreach (var disposable in disposables)
            {
                if (disposable is not Subscription subscription) continue;
                subscription.Invoke();
            }
        }

        public IDisposable Subscribe(Action action) => Subscribe(action, withBuffer: false);

        public IDisposable Subscribe(Action action, bool withBuffer)
        {
            var subscription = new Subscription(action, disposables);
            
            if (disposables.Contains(subscription)) return subscription;
            
            disposables.Add(subscription);
                
            if (withBuffer)
            {
                subscription.Invoke();
            }

            return subscription;
        }

        public override void Dispose()
        {
            disposables.Dispose();
        }
    }

    public abstract partial class GameEvent<T>
    {
        /// <summary>
        /// Raise the event with parameter.
        /// </summary>
        /// /// <param name="param"></param>
        public virtual void Raise(T param)
        {
            _value = param;
            base.Raise();
            foreach (var disposable in disposables)
            {
                if (disposable is not Subscription<T> subscription) continue;
                subscription.Invoke(_value);
            }
        }

        public IDisposable Subscribe(Action<T> action) => Subscribe(action, false);

        public IDisposable Subscribe(Action<T> action, bool withBuffer)
        {
            var subscription = new Subscription<T>(action, disposables);
            
            if (disposables.Contains(subscription)) return subscription;
            
            disposables.Add(subscription);

            if (withBuffer)
            {
                subscription.Invoke(_value);
            }

            return subscription;
        }
    }
#endif
}
