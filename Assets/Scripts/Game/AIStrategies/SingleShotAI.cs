using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class SingleShotAI : ITowerAI
    {
        public Vector2 Position { get; private set; }
        public float Radius { get; private set; }
        public float Delay { get; private set; }
        public float BulletSpeed { get; private set; }
        public event Action<BulletModel> BulletShoot;

        private UnitModel _target;
        private float _lastShotTime;

        public SingleShotAI(float radius, float delay, float bulletSpeed)
        {
            Radius = radius;
            Delay = delay;
            BulletSpeed = bulletSpeed;
            _lastShotTime = delay;
        }

        public void Initialize(Vector2 position)
        {
            Position = position;
        }

        public void SetTarget(UnitModel target)
        {
            _target = target;
        }

        public void ClearTarget()
        {
            _target = null;
        }

        public void Update(float deltaTime)
        {
            _lastShotTime += deltaTime;
            if (_target != null && _lastShotTime >= Delay)
            {
                if (BulletShoot != null)
                    BulletShoot(new BulletModel("bullet1", Position, _target, BulletSpeed));
                _lastShotTime = 0f;
            }
        }
    }
}
