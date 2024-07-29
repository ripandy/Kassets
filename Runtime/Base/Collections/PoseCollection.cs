using UnityEngine;

namespace Kadinche.Kassets.Collection
{
    [CreateAssetMenu(fileName = "PoseCollection", menuName = MenuHelper.DefaultCollectionMenu + "PoseCollection")]
    public sealed class PoseCollection : Collection<Pose>
    {
    }
}