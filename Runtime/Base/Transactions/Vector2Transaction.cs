using UnityEngine;

namespace Kadinche.Kassets.Transaction
{
    [CreateAssetMenu(fileName = "Vector2Transaction", menuName = MenuHelper.DefaultTransactionMenu + "Vector2Transaction")]
    public sealed class Vector2Transaction : TransactionCore<Vector2>
    {
    }
}