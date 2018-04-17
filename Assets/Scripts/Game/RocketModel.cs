using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class RocketModel : BulletModel
    {
        float _startDistance;
        public RocketModel(string visual, Vector2 position, UnitModel target, float speed) : base(visual, position, target, speed)
        {
            _startDistance = Position.DistanceTo(_target.Position);
            _time = Mathf.Sqrt(2 * _startDistance / speed);
        }

        public override void Update(float deltaTime)
        {
            _elapsedTime += deltaTime;

            if (_elapsedTime < _time)
            {
                var direction = (_target.Position - _startPosition);
                Position = _startPosition + direction * _speed * _elapsedTime * _elapsedTime / 2f / _startDistance;
                Direction = (_target.Position - Position).normalized;
            }
            else OnHitted();
        }
    }
}
