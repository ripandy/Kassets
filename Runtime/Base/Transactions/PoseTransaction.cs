using UnityEngine;

namespace Kadinche.Kassets.Transaction
{
    [CreateAssetMenu(fileName = "PoseTransaction", menuName = MenuHelper.DefaultTransactionMenu + "PoseTransaction")]
    public sealed class PoseTransaction : TransactionCore<Pose>
    {
    }
}