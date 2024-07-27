#if KASSETS_R3

using R3;
using UnityEngine;

namespace Kadinche.Kassets.EventSystem
{
    [CreateAssetMenu(fileName = "R3_UnitGameEvent", menuName = MenuHelper.DefaultGameEventMenu + "R3/UnitGameEvent")]
    public class UnitGameEvent : GameEvent<Unit>
    {
    }
}

#endif
