using UnityEngine;

namespace Kadinche.Kassets.EventSystem
{
    [CreateAssetMenu(fileName = "ByteEvent", menuName = MenuHelper.DefaultGameEventMenu + "ByteEvent")]
    public sealed class ByteGameEvent : GameEvent<byte>
    {
    }
}