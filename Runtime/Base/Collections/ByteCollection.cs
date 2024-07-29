using UnityEngine;

namespace Kadinche.Kassets.Collection
{
    [CreateAssetMenu(fileName = "ByteCollection", menuName = MenuHelper.DefaultCollectionMenu + "ByteCollection")]
    public sealed class ByteCollection : Collection<byte>
    {
    }
}