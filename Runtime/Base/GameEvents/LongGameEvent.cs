using UnityEngine;

namespace Kadinche.Kassets.EventSystem
{
    [CreateAssetMenu(fileName = "LongEvent", menuName = MenuHelper.DefaultGameEventMenu + "LongEvent")]
    public sealed class LongGameEvent : GameEvent<long>
    {
    }
}