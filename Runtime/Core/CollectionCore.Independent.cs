#if !KASSETS_R3

using System;
using System.Collections.Generic;
using System.Linq;

namespace Kadinche.Kassets.Collection
{
    public abstract partial class Collection<T>
    {
        private readonly List<IDisposable> _onAddSubscriptions = new();
        private readonly List<IDisposable> _onRemoveSubscriptions = new();
        private readonly List<IDisposable> _onClearSubscriptions = new();
        private readonly Dictionary<int, List<IDisposable>> _valueSubscriptions = new();
        private readonly List<IDisposable> _countSubscriptions = new();
        
        public partial IDisposable SubscribeOnAdd(Action<T> action) => SubscribeOnAdd(action, withBuffer: false);
        public partial IDisposable SubscribeOnRemove(Action<T> action) => SubscribeOnRemove(action, withBuffer: false);
        public partial IDisposable SubscribeOnClear(Action action) => SubscribeOnClear(action, withBuffer: false);
        public partial IDisposable SubscribeToCount(Action<int> action) => SubscribeToCount(action, withBuffer: false);
        public partial IDisposable SubscribeToValueAt(int index, Action<T> action) => SubscribeToValueAt(index, action, withBuffer: false);

        public IDisposable SubscribeOnAdd(Action<T> action, bool withBuffer)
        {
            var subscription = new Subscription<T>(action, _onAddSubscriptions);
            
            if (_onAddSubscriptions.Contains(subscription))
            {
                return subscription;
            }
            
            _onAddSubscriptions.Add(subscription);
            
            if (withBuffer && list.Count > 0)
            {
                subscription.Invoke(list.LastOrDefault());
            }

            return subscription;
        }

        public IDisposable SubscribeOnRemove(Action<T> action, bool withBuffer)
        {
            var subscription = new Subscription<T>(action, _onRemoveSubscriptions);
            
            if (_onRemoveSubscriptions.Contains(subscription))
            {
                return subscription;
            }
            
            _onRemoveSubscriptions.Add(subscription);
            
            if (withBuffer)
            {
                subscription.Invoke(_lastRemoved);
            }

            return subscription;
        }

        public IDisposable SubscribeOnClear(Action action, bool withBuffer)
        {
            var subscription = new Subscription(action, _onClearSubscriptions);
            
            if (_onClearSubscriptions.Contains(subscription))
            {
                return subscription;
            }

            _onClearSubscriptions.Add(subscription);
            
            if (withBuffer)
            {
                subscription.Invoke();
            }

            return subscription;
        }
        
        public IDisposable SubscribeToCount(Action<int> action, bool withBuffer)
        {
            var subscription = new Subscription<int>(action, _countSubscriptions);
            
            if (_countSubscriptions.Contains(subscription))
            {
                return subscription;
            }

            _countSubscriptions.Add(subscription);
            
            if (withBuffer)
            {
                subscription.Invoke(Count);
            }

            return subscription;
        }

        public IDisposable SubscribeToValueAt(int index, Action<T> action, bool withBuffer)
        {
            if (!_valueSubscriptions.TryGetValue(index, out var subscriptions))
            {
                subscriptions = new List<IDisposable>();
                _valueSubscriptions.Add(index, subscriptions);
            }
            
            var subscription = new Subscription<T>(action, subscriptions);
            
            if (subscriptions.Contains(subscription))
            {
                return subscription;
            }

            subscriptions.Add(subscription);
            
            if (withBuffer && index < list.Count && list[index] != null)
            {
                subscription.Invoke(list[index]);
            }

            return subscription;
        }
        
        private partial void RaiseOnAdd(T addedValue)
        {
            foreach (var disposable in _onAddSubscriptions)
            {
                if (disposable is Subscription<T> valueSubscription)
                {
                    valueSubscription.Invoke(addedValue);
                }
            }
        }
        
        private partial void RaiseOnRemove(T removedValue)
        {
            foreach (var disposable in _onRemoveSubscriptions)
            {
                if (disposable is Subscription<T> valueSubscription)
                {
                    valueSubscription.Invoke(removedValue);
                }
            }
        }
        
        private partial void RaiseOnClear()
        {
            foreach (var disposable in _onClearSubscriptions)
            {
                if (disposable is Subscription subscription)
                {
                    subscription.Invoke();
                }
            }
        }
        
        private partial void RaiseCount()
        {
            foreach (var disposable in _countSubscriptions)
            {
                if (disposable is Subscription<int> countSubscription)
                {
                    countSubscription.Invoke(Count);
                }
            }
        }

        private partial void RaiseValueAt(int index, T value)
        {
            if (valueEventType == ValueEventType.ValueChange && list[index].Equals(value))
            {
                return;
            }

            if (!_valueSubscriptions.TryGetValue(index, out var subscriptions))
            {
                return;
            }

            foreach (var disposable in subscriptions)
            {
                if (disposable is Subscription<T> valueSubscription)
                {
                    valueSubscription.Invoke(value);
                }
            }
        }
        
        private partial void IncrementValueSubscriptions(int index)
        {
            for (var i = list.Count; i > index; i--)
            {
                _valueSubscriptions.TryChangeKey(i - 1, i);
            }
        }

        private partial void SwitchValueSubscription(int oldIndex, int newIndex)
        {
            _valueSubscriptions.TryChangeKey(oldIndex, newIndex);
        }
        
        private partial void ClearValueSubscriptions()
        {
            foreach (var subscriptions in _valueSubscriptions.Values)
            {
                for (var i = subscriptions.Count - 1; i >= 0; i--)
                {
                    subscriptions[i].Dispose();
                }
                subscriptions.Clear();
            }
            _valueSubscriptions.Clear();
        }
        
        private partial void RemoveValueSubscription(int index)
        {
            if (!_valueSubscriptions.TryGetValue(index, out var subscriptions)) return;
            subscriptions.Dispose();
            _valueSubscriptions.Remove(index);
        }

        private partial void DisposeSubscriptions()
        {
            _onAddSubscriptions.Dispose();
            _onRemoveSubscriptions.Dispose();
            _onClearSubscriptions.Dispose();
            _countSubscriptions.Dispose();
            ClearValueSubscriptions();
        }
    }
    
    public abstract partial class Collection<TKey, TValue>
    {
        private readonly IDictionary<TKey, IList<IDisposable>> _valueSubscriptions = new Dictionary<TKey, IList<IDisposable>>();

        public IDisposable SubscribeOnAdd(Action<TKey, TValue> action, bool withBuffer) => base.SubscribeOnAdd(pair => action.Invoke(pair.Key, pair.Value), withBuffer);
        public IDisposable SubscribeOnAdd(Action<TKey, TValue> action) => SubscribeOnAdd(action, withBuffer: false);
        
        public IDisposable SubscribeOnRemove(Action<TKey, TValue> action, bool withBuffer) => base.SubscribeOnRemove(pair => action.Invoke(pair.Key, pair.Value), withBuffer);
        public IDisposable SubscribeOnRemove(Action<TKey, TValue> action) => SubscribeOnRemove(action, withBuffer: false);
        
        public partial IDisposable SubscribeToValue(TKey key, Action<TValue> action) => SubscribeToValue(key, action, withBuffer: false);

        public IDisposable SubscribeToValue(TKey key, Action<TValue> action, bool withBuffer)
        {
            if (!_valueSubscriptions.TryGetValue(key, out var subscriptions))
            {
                subscriptions = new List<IDisposable>();
                _valueSubscriptions.Add(key, subscriptions);
            }
            
            var subscription = new Subscription<TValue>(action, subscriptions);
            
            if (subscriptions.Contains(subscription))
            {
                return subscription;
            }

            subscriptions.Add(subscription);
            
            if (withBuffer && _dictionary.TryGetValue(key, out var value) && value != null)
            {
                subscription.Invoke(value);
            }

            return subscription;
        }
        
        private partial void RaiseValue(TKey key, TValue value)
        {
            if (valueEventType == ValueEventType.ValueChange && _dictionary.TryGetValue(key, out var val) && val.Equals(value))
            {
                return;
            }

            if (!_valueSubscriptions.TryGetValue(key, out var subscriptions))
            {
                return;
            }

            foreach (var disposable in subscriptions)
            {
                if (disposable is Subscription<TValue> valueSubscription)
                {
                    valueSubscription.Invoke(value);
                }
            }
        }

        private partial void ClearValueSubscriptions()
        {
            foreach (var subscriptions in _valueSubscriptions.Values)
            {
                foreach (var disposable in subscriptions)
                {
                    disposable.Dispose();
                }
                subscriptions.Clear();
            }
            _valueSubscriptions.Clear();
        }

        private partial void RemoveValueSubscription(TKey key)
        {
            if (!_valueSubscriptions.TryGetValue(key, out var subscriptions))
            {
                return;
            }

            subscriptions.Dispose();
            _valueSubscriptions.Remove(key);
        }
    }
}

#endif