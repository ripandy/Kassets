using UnityEngine;

namespace Kadinche.Kassets.Variable
{
    [CreateAssetMenu(fileName = "TransformVariable", menuName = MenuHelper.DefaultVariableMenu + "Transform")]
    public sealed class TransformVariable : VariableCore<Transform>
    {
    }
}