using UnityEngine;

namespace Kadinche.Kassets.Collection
{
    [CreateAssetMenu(fileName = "LongCollection", menuName = MenuHelper.DefaultCollectionMenu + "LongCollection")]
    public sealed class LongCollection : Collection<long>
    {
    }
}