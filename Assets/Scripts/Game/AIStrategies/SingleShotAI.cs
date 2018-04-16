using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MimiJson;

namespace Game
{
    public class SingleShotAI : ITowerAI
    {
        public float Radius { get; private set; }
        public int Level { get; private set; }
        public event Action<BulletModel> BulletShoot;

        private float _delay;
        private float _bulletSpeed;
        private float _damage;
        private string _bulletVisual;

        private JsonArray _turretData;
        private Vector2 _position;
        private UnitModel _target;
        private float _lastShotTime;
        private BulletModel _bullet;

        public SingleShotAI(JsonArray turretData)
        {
            _turretData = turretData;
            UpgradeToLevel(0);
            _lastShotTime = _delay;
        }

        public void UpgradeToLevel(int level)
        {
            var data = _turretData[level];
            UpgradeToLevel(data);
            Level = level;
        }

        protected virtual void UpgradeToLevel(JsonValue data)
        {
            Radius = data["radius"];
            _delay = data["delay"];
            _bulletSpeed = data["bullet_speed"];
            _damage = data["damage"];
            _bulletVisual = data["bullet_visual"];
        }

        public void Initialize(Vector2 position)
        {
            _position = position;
        }

        public void SetTarget(UnitModel target)
        {
            _target = target;
        }

        public void ClearTarget()
        {
            _target = null;
        }

        public virtual void Update(float deltaTime)
        {
            _lastShotTime += deltaTime;
            if (_target != null && _lastShotTime >= _delay)
            {
                Shot();
                _lastShotTime = 0f;
            }
        }

        protected virtual void Shot()
        {
            _bullet = new BulletModel(_bulletVisual, _position, _target, _bulletSpeed);
            _bullet.Hitted += _bullet_Hitted;
            if (BulletShoot != null)
                BulletShoot(_bullet);
        }

        private void _bullet_Hitted(BulletModel bullet, UnitModel unit)
        {
            unit.Health -= _damage;
        }
    }
}
