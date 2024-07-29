using UnityEngine;

namespace Kadinche.Kassets.Collection
{
    [CreateAssetMenu(fileName = "IntCollection", menuName = MenuHelper.DefaultCollectionMenu + "IntCollection")]
    public sealed class IntCollection : Collection<int>
    {
    }
}