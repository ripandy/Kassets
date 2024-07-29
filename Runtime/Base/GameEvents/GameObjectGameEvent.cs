using UnityEngine;

namespace Kadinche.Kassets.EventSystem
{
    [CreateAssetMenu(fileName = "GameObjectEvent", menuName = MenuHelper.DefaultGameEventMenu + "GameObjectEvent")]
    public sealed class GameObjectGameEvent : GameEvent<GameObject>
    {
    }
}