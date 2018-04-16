﻿using System;
using UnityEngine;

namespace Game
{
    public interface ITowerAI
    {
        Vector2 Position { get; }
        float Radius { get; }
        float Delay { get; }
        event Action<BulletModel> BulletShoot;

        void Update(float deltaTime);
        void SetTarget(UnitModel target);
        void ClearTarget();
        void Initialize(Vector2 position);
    }
}