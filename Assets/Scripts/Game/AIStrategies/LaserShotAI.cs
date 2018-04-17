using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MimiJson;

namespace Game
{
    public class LaserShotAI : SingleShotAI
    {
        private LaserBulletModel _laserBullet;
        public LaserShotAI(JsonArray turretData) : base(turretData)
        {

        }

        protected override void UpgradeToLevel(JsonValue data)
        {
            Radius = data["radius"];
            _damage = data["damage"];
            _bulletVisual = data["bullet_visual"];
        }
        
        public override void ClearTarget()
        {
            base.ClearTarget();
            if (_laserBullet != null)
                _laserBullet.SetTarget(_target);
        }

        public override void Update(float deltaTime)
        {
            if (_target != null)
            {
                if (_bullet == null)
                {
                    _laserBullet = new LaserBulletModel(_bulletVisual, _position, _target, _bulletSpeed);
                    _bullet = _laserBullet;
                    _bullet.Hitted += _bullet_Hitted;
                    OnBulletShoot();
                }
                _target.Health -= _damage * deltaTime;
            }
            _laserBullet.SetTarget(_target);
        }
        
        protected override void _bullet_Hitted(BulletModel bullet, UnitModel unit)
        {

        }
    }
}
