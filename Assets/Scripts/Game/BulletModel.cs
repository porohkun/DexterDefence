using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class BulletModel
    {
        public Vector2 Position { get; protected set; }
        public Vector2 Direction { get; protected set; }
        public string Visual { get; private set; }
        public event Action<BulletModel, UnitModel> Hitted;

        protected Vector2 _startPosition;
        protected UnitModel _target;
        protected float _speed;
        protected float _time;
        protected float _elapsedTime = 0f;

        public BulletModel(string visual, Vector2 position, UnitModel target, float speed)
        {
            Visual = visual;
            _startPosition = position;
            _target = target;
            _speed = speed;

            Position = position + (_target.Position - position).normalized / 2f;
            _startPosition = Position;
            Direction = (_target.Position - Position).normalized;
            _time = Position.DistanceTo(_target.Position) / speed;
        }

        public virtual void Update(float deltaTime)
        {
            _elapsedTime += deltaTime;

            if (_elapsedTime < _time)
            {
                var direction = (_target.Position - _startPosition);
                Position = _startPosition + direction * (_elapsedTime / _time);
                Direction = (_target.Position - Position).normalized;
            }
            else OnHitted();
        }

        protected void OnHitted()
        {
            if (Hitted != null)
                Hitted(this, _target);
        }
    }
}
