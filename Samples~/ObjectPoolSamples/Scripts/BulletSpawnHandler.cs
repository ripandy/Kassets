using System;
using System.Collections;
using Kadinche.Kassets.EventSystem;
using UnityEngine;

namespace Kadinche.Kassets.ObjectPool.Samples
{
    public class BulletSpawnHandler : MonoBehaviour
    {
        [SerializeField] private GameEvent spawnEvent;
        [SerializeField] private TransformPool bulletTransformPool;
        [SerializeField] private float bulletMaxDistance = 20f;
        [SerializeField] private float bulletSpeed = 20f;
        
        private IDisposable subscription;

        private void Start()
        {
            bulletTransformPool.DefaultParent = transform;
            subscription = spawnEvent.Subscribe(Spawn);
        }

        private void Spawn()
        {
            StartCoroutine(SpawnAndMoveRoutine());
        }

        private IEnumerator SpawnAndMoveRoutine()
        {
            using var _ = bulletTransformPool.Get(out var bullet);

            var startingPos = bullet.position;
            while (Vector3.Distance(bullet.position, startingPos) < bulletMaxDistance)
            {
                bullet.position += bullet.forward * (bulletSpeed * Time.deltaTime);
                yield return null;
            }

            bullet.position = startingPos;
        }

        private void OnDestroy()
        {
            subscription?.Dispose();
        }
    }
}
