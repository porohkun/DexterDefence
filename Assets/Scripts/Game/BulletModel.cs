using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class BulletModel
    {
        public Vector2 Position { get; private set; }
        public Vector2 Direction { get; private set; }
        public string Visual { get; private set; }
        public event Action<BulletModel, UnitModel> Hitted;

        private Vector2 _startPosition;
        private UnitModel _target;
        private float _speed;
        private float _time;
        private float _elapsedTime = 0f;

        public BulletModel(string visual, Vector2 position, UnitModel target, float speed)
        {
            Visual = visual;
            _startPosition = position;
            _target = target;
            _speed = speed;

            Position = position + (_target.Position - position).normalized / 2f;
            _time = Position.DistanceTo(_target.Position) / speed;
        }

        public void Update(float deltaTime)
        {
            _elapsedTime += deltaTime;

            if (_elapsedTime < _time)
            {
                var direction = (_target.Position - _startPosition);
                Position = _startPosition + direction * (_elapsedTime / _time);
                Direction = (_target.Position - Position).normalized;
            }
            else if (Hitted != null)
                Hitted(this, _target);
        }
    }
}
