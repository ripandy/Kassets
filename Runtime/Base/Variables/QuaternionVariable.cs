using UnityEngine;

namespace Kadinche.Kassets.Variable
{
    [CreateAssetMenu(fileName = "QuaternionVariable", menuName = MenuHelper.DefaultVariableMenu + "Quaternion")]
    public sealed class QuaternionVariable : VariableCore<Quaternion>
    {
    }
}