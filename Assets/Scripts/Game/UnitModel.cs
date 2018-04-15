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
        public Vector2 Direction { get; private set; }
        public float Speed { get; private set; }
        public float Health { get; private set; }
        public event System.Action<UnitModel> Died;
        public event System.Action<UnitModel> Finished;

        private MapModel _map;
        private Vector2 _targetPosition;
        private CellModel _target;

        private IEnumerator<bool> _currentMove = null;
        private float _lastdeltaTime;

        public UnitModel(string visual, int health, float speed)
        {
            Visual = visual;
            Health = health;
            Speed = speed;
        }

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
                Random.Range(position.X - 0.45f, position.X + 0.45f),
                Random.Range(position.Y - 0.45f, position.Y + 0.45f)
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
            if (!_currentMove.Current)
            {
                if (_target == null)
                {
                    if (Finished != null)
                        Finished(this);
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
            Direction = (end - start).normalized;
            var time = distance / Speed;
            var elapsedTime = 0f;

            while (true)
            {
                elapsedTime += _lastdeltaTime;
                Position = Vector2.Lerp(start, end, Mathf.Min(elapsedTime / time, 1f));
                if (elapsedTime < time)
                    yield return true;
                else
                {
                    _lastdeltaTime = elapsedTime - time;
                    break;
                }
            }
            yield return false;
        }
    }
}
