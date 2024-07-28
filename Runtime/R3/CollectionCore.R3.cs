#if KASSETS_R3

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using R3;

namespace Kadinche.Kassets.Collection
{
    public abstract partial class Collection<T>
    {
        private CancellationTokenSource cts;
        protected CancellationToken DefaultToken
        {
            get
            {
                cts ??= new CancellationTokenSource();
                return cts.Token;
            }
        }

        private readonly Subject<T> _onAddSubject = new();
        private readonly Subject<T> _onRemoveSubject = new();
        private readonly Subject<object> _onClearSubject = new();
        private readonly Subject<int> _countSubject = new();
        private readonly Dictionary<int, Subject<T>> _valueSubjects = new();

        public Observable<T> OnAddObservable() => _onAddSubject;
        public Observable<T> OnRemoveObservable() => _onRemoveSubject;
        public Observable<object> OnClearObservable() => _onClearSubject;
        public Observable<int> CountObservable() => _countSubject;

        public Observable<T> ValueAtObservable(int index)
        {
            if (_valueSubjects.TryGetValue(index, out var elementSubject)) return elementSubject;
            
            elementSubject = new Subject<T>();
            _valueSubjects.Add(index, elementSubject);

            return elementSubject;
        }
        
        public async ValueTask<T> OnAddAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DefaultToken);
            return await _onAddSubject.LastOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public async ValueTask<T> OnRemoveAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DefaultToken);
            return await _onRemoveSubject.LastOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public async ValueTask OnClearAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DefaultToken);
            await _onClearSubject.WaitAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public async ValueTask<int> CountAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DefaultToken);
            return await _countSubject.LastOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
        
        public async ValueTask<T> ValueAtAsync(int index, CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DefaultToken);
            return await ValueAtObservable(index).LastOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
    }

    public abstract partial class Collection<TKey, TValue>
    {
        private readonly Dictionary<TKey, Subject<TValue>> _valueSubjects = new();
        
        public Observable<TValue> ValueAtObservable(TKey key)
        {
            if (_valueSubjects.TryGetValue(key, out var elementSubject)) return elementSubject;
            
            elementSubject = new Subject<TValue>();
            _valueSubjects.Add(key, elementSubject);

            return elementSubject;
        }
        
        public async ValueTask<TValue> ValueAtAsync(TKey key, CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, DefaultToken);
            return await ValueAtObservable(key).LastOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }
    }
    
    public abstract partial class Collection<T>
    {
        private partial void RaiseOnAdd(T addedValue)
        {
            _onAddSubject.OnNext(addedValue);
        }

        private partial void RaiseOnRemove(T removedValue)
        {
            _onRemoveSubject.OnNext(removedValue);
        }

        private partial void RaiseOnClear()
        {
            _onClearSubject.OnNext(this);
        }

        private partial void RaiseCount()
        {
            _countSubject.OnNext(Count);
        }

        private partial void RaiseValueAt(int index, T value)
        {
            if (valueEventType == ValueEventType.ValueChange && list[index].Equals(value))
            {
                return;
            }

            if (_valueSubjects.TryGetValue(index, out var subject))
            {
                subject.OnNext(value);
            }
        }

        public partial IDisposable SubscribeOnAdd(Action<T> action)
        {
            return _onAddSubject.Subscribe(action);
        }

        public partial IDisposable SubscribeOnRemove(Action<T> action)
        {
            return _onRemoveSubject.Subscribe(action);
        }

        public partial IDisposable SubscribeOnClear(Action action)
        {
            return _onClearSubject.Subscribe(_ => action.Invoke());
        }
        
        public partial IDisposable SubscribeToCount(Action<int> action)
        {
            return _countSubject.Subscribe(action);
        }

        public partial IDisposable SubscribeToValueAt(int index, Action<T> action)
        {
            return ValueAtObservable(index).Subscribe(action);
        }
        
        private partial void IncrementValueSubscriptions(int index)
        {
            for (var i = list.Count; i > index; i--)
            {
                _valueSubjects.TryChangeKey(i - 1, i);
            }
        }
        
        private partial void SwitchValueSubscription(int oldIndex, int newIndex)
        {
            _valueSubjects.TryChangeKey(oldIndex, newIndex);
        }
        
        private partial void ClearValueSubscriptions()
        {
            foreach (var subject in _valueSubjects.Values)
            {
                subject.Dispose();
            }
            _valueSubjects.Clear();
        }
        
        private partial void RemoveValueSubscription(int index)
        {
            if (!_valueSubjects.TryGetValue(index, out var subject)) return;
            subject.Dispose();
            _valueSubjects.Remove(index);
        }

        private partial void DisposeSubscriptions()
        {
            _onAddSubject.Dispose();
            _onRemoveSubject.Dispose();
            _onClearSubject.Dispose();
            _countSubject.Dispose();
            ClearValueSubscriptions();
            cts?.CancelAndDispose();
            cts = null;
        }
    }
    
    public abstract partial class Collection<TKey, TValue>
    {
        public partial IDisposable SubscribeToValue(TKey key, Action<TValue> action)
        {
            return ValueAtObservable(key).Subscribe(action);
        }

        private partial void RaiseValue(TKey key, TValue value)
        {
            if (valueEventType == ValueEventType.ValueChange && _dictionary.TryGetValue(key, out var value2) && value2.Equals(value))
            {
                return;
            }
            
            if (_valueSubjects.TryGetValue(key, out var subject))
            {
                subject.OnNext(value);
            }
        }

        private partial void ClearValueSubscriptions()
        {
            foreach (var subject in _valueSubjects.Values)
            {
                subject.Dispose();
            }
            _valueSubjects.Clear();
        }
        
        private partial void RemoveValueSubscription(TKey key)
        {
            if (!_valueSubjects.TryGetValue(key, out var subject)) return;
            subject.Dispose();
            _valueSubjects.Remove(key);
        }
    }
}

#endif