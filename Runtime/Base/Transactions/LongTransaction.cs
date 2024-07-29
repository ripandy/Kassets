using UnityEngine;

namespace Kadinche.Kassets.Transaction
{
    [CreateAssetMenu(fileName = "LongTransaction", menuName = MenuHelper.DefaultTransactionMenu + "LongTransaction")]
    public sealed class LongTransaction : TransactionCore<long>
    {
    }
}