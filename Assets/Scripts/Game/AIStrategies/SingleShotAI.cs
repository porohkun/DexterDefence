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
        public bool CanBeUpgraded { get { return Level < _turretData.Length - 1; } }
        public int UpgradeCost { get { return _turretData[Level + 1]["cost"]; } }
        public int Cashback { get { return _turretData[Level]["cashback"]; } }

        public event Action<BulletModel> BulletShoot;

        protected float _delay;
        protected float _bulletSpeed;
        protected float _damage;
        protected string _bulletVisual;

        protected JsonArray _turretData;
        protected Vector2 _position;
        protected UnitModel _target;
        protected float _lastShotTime;
        protected BulletModel _bullet;

        public SingleShotAI(JsonArray turretData)
        {
            _turretData = turretData;
            UpgradeToLevel(0);
            _lastShotTime = _delay;
        }

        public void UpgradeTower()
        {
            UpgradeToLevel(Level + 1);
        }

        private void UpgradeToLevel(int level)
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
            OnBulletShoot();
        }

        protected void OnBulletShoot()
        {
            if (BulletShoot != null)
                BulletShoot(_bullet);
        }

        protected virtual void _bullet_Hitted(BulletModel bullet, UnitModel unit)
        {
            unit.Health -= _damage;
        }
    }
}
