﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kadinche.Kassets.EventSystem
{
    /// <summary>
    /// Core Game Event System.
    /// </summary>
    [CreateAssetMenu(fileName = "GameEvent", menuName = MenuHelper.DefaultEventMenu + "GameEvent")]
    public class GameEvent : KassetsBase, IEventRaiser, IEventHandler
    {
        [Tooltip("Whether to listen to previous event upon subscription.")]
        [SerializeField] protected bool buffered;
        
        public IDisposable Subscribe(Action action) => Subscribe(action, buffered);
        
        public void Request(Action onResponse)
        {
            using (Subscribe(onResponse))
            {
                Raise();   
            }
        }
        
        protected readonly IList<IDisposable> disposables = new List<IDisposable>();

        /// <summary>
        /// Raise the event.
        /// </summary>
        public virtual void Raise()
        {
            foreach (var disposable in disposables)
            {
                if (disposable is Subscription subscription)
                {
                    subscription.Invoke();
                }
            }
        }

        public IDisposable Subscribe(Action action, bool withBuffer)
        {
            var subscription = new Subscription(action, disposables);
            if (!disposables.Contains(subscription))
            {
                disposables.Add(subscription);
                
                if (withBuffer)
                {
                    subscription.Invoke();
                }
            }

            return subscription;
        }

        public override void Dispose() => disposables.Dispose();
    }

    /// <summary>
    /// Generic base class for event system with parameter.
    /// </summary>
    /// <typeparam name="T">Parameter type for the event system</typeparam>
    public abstract class GameEvent<T> : GameEvent, IEventRaiser<T>, IEventHandler<T>
    {
        [SerializeField] protected T _value;

        public override void Raise() => Raise(_value);

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
                if (disposable is Subscription<T> subscription)
                {
                    subscription.Invoke(_value);
                }
            }
        }

        public IDisposable Subscribe(Action<T> action) => Subscribe(action, buffered);
        
        public IDisposable Subscribe(Action<T> action, bool withBuffer)
        {
            var subscription = new Subscription<T>(action, disposables);
            if (!disposables.Contains(subscription))
            {
                disposables.Add(subscription);
                
                if (withBuffer)
                {
                    subscription.Invoke(_value);
                }
            }

            return subscription;
        }
        
        public override void OnAfterDeserialize()
        {
            _value = default;
        }
    }
    
    /// <summary>
    /// An event that contains collection of events. Get raised whenever any event is raised.
    /// Made it possible to listen to many events at once.
    /// </summary>
    [Serializable]
    public class GameEventCollection : IEventRaiser, IEventHandler, IDisposable
    {
        [SerializeField] private List<GameEvent> _gameEvents;
        [SerializeField] protected bool buffered;

        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public IDisposable Subscribe(Action action) => Subscribe(action, buffered);

        public IDisposable Subscribe(Action onAnyEvent, bool withBuffer)
        {
            foreach (var gameEvent in _gameEvents)
            {
                _compositeDisposable.Add(gameEvent.Subscribe(onAnyEvent));
            }

            if (withBuffer)
            {
                onAnyEvent.Invoke();
            }

            return _compositeDisposable;
        }

        public void Raise()
        {
            foreach (var gameEvent in _gameEvents)
            {
                gameEvent.Raise();
            }
        }

        public void Request(Action onResponse)
        {
            using (Subscribe(onResponse))
            {
                Raise();
            }
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }
    }
}