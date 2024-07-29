using UnityEngine;

namespace Kadinche.Kassets.EventSystem
{
    [CreateAssetMenu(fileName = "QuaternionEvent", menuName = MenuHelper.DefaultGameEventMenu + "QuaternionEvent")]
    public sealed class QuaternionGameEvent : GameEvent<Quaternion>
    {
    }
}