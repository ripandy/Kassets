using Kadinche.Kassets.EventSystem;
using UnityEngine;

namespace Kadinche.Kassets.ObjectPool.Samples
{
    public class ShootInputProvider : MonoBehaviour
    {
        [SerializeField] private GameEvent shootBulletEvent;
        [SerializeField] private KeyCode shootKey;

        private void Update()
        {
            if (Input.GetKeyDown(shootKey)) 
                shootBulletEvent.Raise();
        }
    }
}