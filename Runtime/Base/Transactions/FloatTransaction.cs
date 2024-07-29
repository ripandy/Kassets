using UnityEngine;

namespace Kadinche.Kassets.Transaction
{
    [CreateAssetMenu(fileName = "FloatTransaction", menuName = MenuHelper.DefaultTransactionMenu + "FloatTransaction")]
    public sealed class FloatTransaction : TransactionCore<float>
    {
    }
}