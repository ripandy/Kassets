using UnityEngine;

namespace Kadinche.Kassets.EventSystem
{
    [CreateAssetMenu(fileName = "DoubleEvent", menuName = MenuHelper.DefaultGameEventMenu + "DoubleEvent")]
    public sealed class DoubleGameEvent : GameEvent<double>
    {
    }
}