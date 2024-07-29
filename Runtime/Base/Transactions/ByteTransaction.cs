using UnityEngine;

namespace Kadinche.Kassets.Transaction
{
    [CreateAssetMenu(fileName = "ByteTransaction", menuName = MenuHelper.DefaultTransactionMenu + "ByteTransaction")]
    public sealed class ByteTransaction : TransactionCore<byte>
    {
    }
}