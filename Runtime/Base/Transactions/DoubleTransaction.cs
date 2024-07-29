using UnityEngine;

namespace Kadinche.Kassets.Transaction
{
    [CreateAssetMenu(fileName = "DoubleTransaction", menuName = MenuHelper.DefaultTransactionMenu + "DoubleTransaction")]
    public sealed class DoubleTransaction : TransactionCore<double>
    {
    }
}