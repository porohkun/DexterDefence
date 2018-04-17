using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MimiJson;

namespace Game
{
    public class RocketShotAI : SingleShotAI
    {
        private float _explosionRadius;
        private Func<IEnumerable<UnitModel>> _getUnits;

        public RocketShotAI(JsonArray turretData, Func<IEnumerable<UnitModel>> getUnitsCallback) : base(turretData)
        {
            _getUnits = getUnitsCallback;
        }

        protected override void UpgradeToLevel(JsonValue data)
        {
            base.UpgradeToLevel(data);
            _explosionRadius = data["explosion_radius"];
        }

        protected override void Shot()
        {
            _bullet = new RocketModel(_bulletVisual, _position, _target, _bulletSpeed);
            _bullet.Hitted += _bullet_Hitted;
            OnBulletShoot();
        }

        protected override void _bullet_Hitted(BulletModel bullet, UnitModel _)
        {
            foreach (var unit in _getUnits())
                if (unit.Position.x >= bullet.Position.x - _explosionRadius && unit.Position.x <= bullet.Position.x + _explosionRadius &&
                    unit.Position.y >= bullet.Position.y - _explosionRadius && unit.Position.y <= bullet.Position.y + _explosionRadius)
                    if (unit.Position.DistanceTo(bullet.Position) < _explosionRadius)
                        unit.Health -= _damage;
        }
    }
}
