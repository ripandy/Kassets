using UnityEngine;

namespace Kadinche.Kassets.Variable
{
    [CreateAssetMenu(fileName = "GameObjectVariable", menuName = MenuHelper.DefaultVariableMenu + "GameObject")]
    public sealed class GameObjectVariable : VariableCore<GameObject>
    {
    }
}