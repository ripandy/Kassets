using UnityEngine;

namespace Kadinche.Kassets.Variable
{
    [CreateAssetMenu(fileName = "PoseVariable", menuName = MenuHelper.DefaultVariableMenu + "Pose")]
    public sealed class PoseVariable : VariableCore<Pose>
    {
    }
}