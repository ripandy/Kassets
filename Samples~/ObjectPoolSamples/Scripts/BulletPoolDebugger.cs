using TMPro;
using UnityEngine;

namespace Kadinche.Kassets.ObjectPool.Samples
{
    public class BulletPoolDebugger : MonoBehaviour
    {
        [SerializeField] private TransformPool bulletTransformPool;
        [SerializeField] private TMP_Text debugText;

        private void Update()
        {
            debugText.text = $"Object{(bulletTransformPool.CountInactive > 1 ? "s" : "")} in Pool: {bulletTransformPool.CountInactive}";
        }
    }
}
