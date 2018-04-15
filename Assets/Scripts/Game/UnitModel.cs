using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using StartPosition = System.Collections.Generic.KeyValuePair<Game.Point, Game.Direction>;

namespace Game
{
    public class UnitModel
    {
        public string Visual { get; private set; }
        public Vector2 Position { get; private set; }
        public float Speed { get; private set; }
        public float Health { get; private set; }
        public event System.Action Finished;

        private MapModel _map;
        private Vector2 _targetPosition;
        private CellModel _target;

        private IEnumerator<bool> _currentMove = null;
        private float _lastdeltaTime;

        public void Initialize(MapModel map, StartPosition startPosition)
        {
            _map = map;
            Position = startPosition.Key;
            _target = _map[startPosition.Key.AddDirection(startPosition.Value)];
            CalcTargetPosition(_target.Position);
        }

        private void CalcTargetPosition(Point position)
        {
            _targetPosition = new Vector2(
                Random.Range(position.X - 0.25f, position.X + 0.25f),
                Random.Range(position.Y - 0.25f, position.Y + 0.25f)
                );
        }

        public void Update(float deltaTime)
        {
            _lastdeltaTime = deltaTime;
            if (_currentMove == null)
                _currentMove = MoveRoutine();
            Move();
        }

        private void Move()
        {
            _currentMove.MoveNext();
            if (_currentMove.Current)
            {
                if (_target == null)
                {
                    if (Finished != null)
                        Finished();
                    return;
                }
                var wp = _target.GetNextWaypoint();
                var nextPos = _target.Position.AddDirection(wp);
                CalcTargetPosition(nextPos);
                if (_map.CorrectPosition(nextPos))
                    _target = _map[nextPos];
                else
                    _target = null;
                _currentMove = MoveRoutine();
                Move();
            }
        }

        private IEnumerator<bool> MoveRoutine()
        {
            var start = Position;
            var end = _targetPosition;

            var distance = end.DistanceTo(start);
            var direction = end - start;
            var time = distance / Speed;
            var elapsedTime = 0f;

            while (true)
            {
                elapsedTime += _lastdeltaTime;
                Position = start + direction * Mathf.Min(time, elapsedTime);
                if (elapsedTime < time)
                    yield return true;
                else
                {
                    _lastdeltaTime = elapsedTime - time;
                }
            }
            yield return false;
        }
    }
}
