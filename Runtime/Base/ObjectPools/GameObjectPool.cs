#if UNITY_2021_1_OR_NEWER

using UnityEngine;

namespace Kadinche.Kassets.ObjectPool
{
    [CreateAssetMenu(fileName = "NewGameObjectPool", menuName = MenuHelper.DefaultObjectPoolMenu + "GameObjectPool")]
    public sealed class GameObjectPool : ObjectPoolCore<GameObject>
    {
        [SerializeField] private GameObject targetPrefab;

        public Transform DefaultParent { get; set; } = null;
        
        protected override GameObject CreatePooledItem()
        {
            var go = Instantiate(targetPrefab, DefaultParent);
            return go;
        }

        protected override void OnTakeFromPool(GameObject go) => go.SetActive(true);
        protected override void OnReturnedToPool(GameObject go) => go.SetActive(false);
        protected override void OnDestroyPoolObject(GameObject go) => Destroy(go);
    }
}

#endif