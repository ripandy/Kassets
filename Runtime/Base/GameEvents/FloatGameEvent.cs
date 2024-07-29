using UnityEngine;

namespace Kadinche.Kassets.EventSystem
{
    [CreateAssetMenu(fileName = "FloatEvent", menuName = MenuHelper.DefaultGameEventMenu + "FloatEvent")]
    public sealed class FloatGameEvent : GameEvent<float>
    {
    }
}