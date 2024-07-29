/*
 implementation referenced directly from official unity documentation
 https://docs.unity3d.com/ScriptReference/Pool.ObjectPool_1.html
*/

#if UNITY_2021_1_OR_NEWER

using UnityEngine;
using UnityEngine.Pool;

namespace Kadinche.Kassets
{
    public abstract class ObjectPoolCore<T> : KassetsCore, IObjectPool<T> where T : class
    {
        private enum PoolType
        {
            Stack,
            LinkedList
        }

        [SerializeField] private PoolType poolType;

        [Tooltip("Collection checks will throw errors if we try to release an item that is already in the pool.")]
        [SerializeField] private bool collectionChecks = true;
        [SerializeField] private int maxPoolSize = 10;

        private IObjectPool<T> pool;

        public IObjectPool<T> Pool
        {
            get
            {
                return pool ??= poolType switch
                {
                    PoolType.Stack => new ObjectPool<T>
                    (
                        CreatePooledItem, 
                        OnTakeFromPool, 
                        OnReturnedToPool,
                        OnDestroyPoolObject, 
                        collectionChecks, 
                        10, 
                        maxPoolSize
                    ),
                    PoolType.LinkedList => new LinkedPool<T>
                    (
                        CreatePooledItem, 
                        OnTakeFromPool, 
                        OnReturnedToPool,
                        OnDestroyPoolObject, 
                        collectionChecks, 
                        maxPoolSize
                    ),
                    _ => pool
                };
            }
        }

        public void OnValidate()
        {
            if (poolType == PoolType.LinkedList && pool is ObjectPool<T> ||
                poolType == PoolType.Stack && pool is LinkedPool<T>)
            {
                Dispose();
            }
            
            if (pool == null) _ = Pool;
        }

        protected abstract T CreatePooledItem();
        protected virtual void OnTakeFromPool(T obj) {}
        protected virtual void OnReturnedToPool(T obj) {}
        protected virtual void OnDestroyPoolObject(T obj) {}

        public T Get() => Pool.Get();
        public PooledObject<T> Get(out T v) => Pool.Get(out v);
        public void Release(T element) => Pool.Release(element);
        public void Clear() => Pool.Clear();
        public int CountInactive => Pool.CountInactive;
        
        public override void Dispose()
        {
            pool?.Clear();
            pool = null;
        }
    }
}

#endif