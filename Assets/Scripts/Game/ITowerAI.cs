using System;
using UnityEngine;

namespace Game
{
    public interface ITowerAI
    {
        float Radius { get; }
        int Level { get; }
        bool CanBeUpgraded { get; }
        int UpgradeCost { get; }
        int Cashback { get; }

        event Action<BulletModel> BulletShoot;

        void Update(float deltaTime);
        void SetTarget(UnitModel target);
        void ClearTarget();
        void Initialize(Vector2 position);
        void UpgradeTower();
    }
}