using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kadinche.Kassets.Collection
{
    public abstract partial class Collection<T> : KassetsCore, IList<T>, IReadOnlyList<T>
    {
        [SerializeField] protected List<T> list = new();
        
        [Tooltip("Set how value event behave.\nValue Assign: Raise when value is assigned regardless of value.\nValue Changed: Raise only when value is changed.")]
        [SerializeField] protected ValueEventType valueEventType;
        
        [Tooltip("If true will reset value(s) when play mode end. Otherwise, keep runtime value. Due to shallow copying of class types, it is better avoid using autoResetValue on Class type.")]
        [SerializeField] protected bool autoResetValue;
        
        private readonly List<T> _initialValue = new();
        private readonly object _syncRoot = new();
        private T _lastRemoved;
        
        public T this[int index]
        {
            get
            {
                lock (_syncRoot)
                {
                    return list[index];
                }
            }
            set
            {
                lock (_syncRoot)
                {
                    list[index] = value;
                    RaiseValueAt(index, value);
                }
            }
        }
        
        protected List<T> InitialValue
        {
            get
            {
                lock (_syncRoot)
                {
                    return _initialValue;
                }
            }
            set
            {
                lock (_syncRoot)
                {
                    _initialValue.Clear();
                    if (value == null) return;
                    _initialValue.AddRange(value);
                }
            }
        }

        public int Count
        {
            get
            {
                lock (_syncRoot)
                {
                    return list.Count;
                }
            }
        }

        public bool IsReadOnly => false;

        public virtual void Add(T item)
        {
            lock (_syncRoot)
            {
                var index = list.Count;
                list.Add(item);
                RaiseOnAdd(item);
                RaiseValueAt(index, item);
                RaiseCount();
            }
        }

        public virtual void AddRange(IEnumerable<T> items)
        {
            AddRange(items.ToArray());
        }

        public virtual void AddRange(T[] items)
        {
            lock (_syncRoot)
            {
                foreach (var item in items)
                {
                    var index = list.Count;
                    list.Add(item);
                    RaiseOnAdd(item);
                    RaiseValueAt(index, item);
                    RaiseCount();
                }
            }
        }
        
        public virtual void Clear()
        {
            lock (_syncRoot)
            {
                ClearValueSubscriptions();
                list.Clear();
                RaiseOnClear();
                RaiseCount();
            }
        }
        
        public bool Contains(T item)
        {
            lock (_syncRoot)
            {
                return list.Contains(item);
            }
        }

        public virtual void Copy(IEnumerable<T> others)
        {
            lock (_syncRoot)
            {
                list.Clear();
                list.AddRange(others);
            }
        }
        
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_syncRoot)
            {
                list.CopyTo(array, arrayIndex);
            }
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            lock (_syncRoot)
            {
                foreach (var item in list)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public void ForEach(Action<T> action)
        {
            lock (_syncRoot)
            {
                foreach (var item in list)
                {
                    action(item);
                }
            }
        }
        
        public int IndexOf(T item)
        {
            lock (_syncRoot)
            {
                return list.IndexOf(item);
            }
        }
        
        public virtual void Insert(int index, T item)
        {
            lock (_syncRoot)
            {
                list.Insert(index, item);
                IncrementValueSubscriptions(index);
                RaiseOnAdd(item);
                RaiseCount();
            }
        }
        
        public void InsertRange(int index, T[] items)
        {
            lock (_syncRoot)
            {
                for (var i = 0; i < items.Length; i++)
                {
                    var idx = i + index;
                    var item = items[i];
                    list.Insert(idx, item);
                    IncrementValueSubscriptions(index);
                    RaiseOnAdd(item);
                    RaiseCount();
                }
            }
        }
        
        public void InsertRange(int index, IEnumerable<T> items)
        {
            InsertRange(index, items.ToArray());
        }
        
        public virtual bool Remove(T item)
        {
            lock (_syncRoot)
            {
                var index = list.IndexOf(item);
                if (index < 0)
                {
                    return false;
                }
                
                _lastRemoved = list[index];
                list.RemoveAt(index);
                RaiseOnRemove(_lastRemoved);
                RemoveValueSubscription(index);
                RaiseCount();
                return true;
            }
        }

        public virtual void RemoveAt(int index)
        {
            lock (_syncRoot)
            {
                _lastRemoved = list[index];
                list.RemoveAt(index);
                RaiseOnRemove(_lastRemoved);
                RemoveValueSubscription(index);
                RaiseCount();
            }
        }

        public virtual void Move(int oldIndex, int newIndex)
        {
            lock (_syncRoot)
            {
                var removedItem = list[oldIndex];
                list.RemoveAt(oldIndex);
                list.Insert(newIndex, removedItem);
                SwitchValueSubscription(oldIndex, newIndex);
            }
        }
        
        protected override void Initialize()
        {
            InitialValue = list;
            base.Initialize();
        }
        
        /// <summary>
        /// Reset value(s) to InitialValue
        /// </summary>
        public void ResetValues() => Copy(InitialValue);
        
        private void ResetInternal()
        {
            if (!autoResetValue) return;
            ResetValues();
        }
        
        protected override void OnQuit()
        {
            ResetInternal();
            base.OnQuit();
        }

        public override void Dispose()
        {
            DisposeSubscriptions();
        }
        
        // List of Partial methods. Implemented in each respective integrated Library.
        public partial IDisposable SubscribeOnAdd(Action<T> action);
        public partial IDisposable SubscribeOnRemove(Action<T> action);
        public partial IDisposable SubscribeOnClear(Action action);
        public partial IDisposable SubscribeToCount(Action<int> action);
        public partial IDisposable SubscribeToValueAt(int index, Action<T> action);
        
        private partial void RaiseOnAdd(T addedValue);
        private partial void RaiseOnRemove(T removedValue);
        private partial void RaiseCount();
        private partial void RaiseOnClear();
        private partial void RaiseValueAt(int index, T value);
        
        private partial void IncrementValueSubscriptions(int index);
        private partial void SwitchValueSubscription(int oldIndex, int newIndex);
        
        private partial void RemoveValueSubscription(int index);
        private partial void ClearValueSubscriptions();
        
        private partial void DisposeSubscriptions();
    }
}