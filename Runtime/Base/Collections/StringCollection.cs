using UnityEngine;

namespace Kadinche.Kassets.Collection
{
    [CreateAssetMenu(fileName = "StringCollection", menuName = MenuHelper.DefaultCollectionMenu + "StringCollection")]
    public sealed class StringCollection : Collection<string>
    {
    }
}