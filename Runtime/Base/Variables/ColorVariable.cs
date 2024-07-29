using UnityEngine;

namespace Kadinche.Kassets.Variable
{
    [CreateAssetMenu(fileName = "ColorVariable", menuName = MenuHelper.DefaultVariableMenu + "Color")]
    public sealed class ColorVariable : VariableCore<Color>
    {
    }
}