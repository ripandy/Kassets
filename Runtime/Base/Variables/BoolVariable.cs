using UnityEngine;

namespace Kadinche.Kassets.Variable
{
    [CreateAssetMenu(fileName = "BoolVariable", menuName = MenuHelper.DefaultVariableMenu + "Bool")]
    public sealed class BoolVariable : VariableCore<bool>
    {
        public void ToggleValue() => Value = !Value;
    }
}