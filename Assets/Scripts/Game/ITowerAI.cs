using System;
using UnityEngine;

namespace Game
{
    public interface ITowerAI
    {
        float Radius { get; }
        int Level { get; }
        event Action<BulletModel> BulletShoot;

        void Update(float deltaTime);
        void SetTarget(UnitModel target);
        void ClearTarget();
        void Initialize(Vector2 position);
    }
}