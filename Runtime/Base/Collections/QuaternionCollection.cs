using UnityEngine;

namespace Kadinche.Kassets.Collection
{
    [CreateAssetMenu(fileName = "QuaternionCollection", menuName = MenuHelper.DefaultCollectionMenu + "QuaternionCollection")]
    public sealed class QuaternionCollection : Collection<Quaternion>
    {
    }
}