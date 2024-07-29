﻿using UnityEngine;

namespace Kadinche.Kassets.EventSystem
{
    [CreateAssetMenu(fileName = "Vector2Event", menuName = MenuHelper.DefaultGameEventMenu + "Vector2Event")]
    public sealed class Vector2GameEvent : GameEvent<Vector2>
    {
    }
}