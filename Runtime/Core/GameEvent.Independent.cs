#if !KASSETS_R3

using System;
using System.Collections.Generic;

namespace Kadinche.Kassets.EventSystem
{
    public partial class GameEvent
    {
        protected readonly IList<IDisposable> Disposables = new List<IDisposable>();

        /// <summary>
        /// Raise the event.
        /// </summary>
        public virtual void Raise()
        {
            foreach (var disposable in Disposables)
            {
                if (disposable is Subscription subscription)
                {
                    subscription.Invoke();
                }
            }
        }

        public IDisposable Subscribe(Action action) => Subscribe(action, withBuffer: false);

        public IDisposable Subscribe(Action action, bool withBuffer)
        {
            var subscription = new Subscription(action, Disposables);
            
            if (Disposables.Contains(subscription))
            {
                return subscription;
            }
            
            Disposables.Add(subscription);
                
            if (withBuffer)
            {
                subscription.Invoke();
            }

            return subscription;
        }

        public override void Dispose()
        {
            Disposables.Dispose();
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
            value = param;
            
            base.Raise();
            
            foreach (var disposable in Disposables)
            {
                if (disposable is Subscription<T> subscription)
                {
                    subscription.Invoke(value);
                }
            }
        }

        public IDisposable Subscribe(Action<T> action)
        {
            return Subscribe(action, withBuffer: false);
        }

        public IDisposable Subscribe(Action<T> action, bool withBuffer)
        {
            var subscription = new Subscription<T>(action, Disposables);
            
            if (Disposables.Contains(subscription))
            {
                return subscription;
            }

            Disposables.Add(subscription);

            if (withBuffer)
            {
                subscription.Invoke(value);
            }

            return subscription;
        }
    }
}

#endif
