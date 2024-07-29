using UnityEngine;

namespace Kadinche.Kassets.Variable
{
    [CreateAssetMenu(fileName = "StringVariable", menuName = MenuHelper.DefaultVariableMenu + "String")]
    public sealed class StringVariable : VariableCore<string>
    {
    }
}