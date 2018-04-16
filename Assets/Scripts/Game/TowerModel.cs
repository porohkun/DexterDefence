using System;
using UnityEngine;

namespace Game
{
    public class TowerModel
    {
        public Shell ShellType { get; private set; }
        public ITowerAI TowerAI { get; private set; }
        public float RotateSpeed { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 Direction { get; private set; }

        private MapModel _map;
        private UnitModel _target;

        public TowerModel(Shell shellType, ITowerAI towerAI, float rotateSpeed)
        {
            ShellType = shellType;
            TowerAI = towerAI;
            RotateSpeed = rotateSpeed;
        }

        public void Initialize(MapModel map, Point position)
        {
            _map = map;
            Position = position;
        }

        public void Update(float deltaTime)
        {
            if (_target != null)
                if (Position.DistanceTo(_target.Position) > TowerAI.Radius)
                    _target = null;
            if (_target == null)
            {
                var left = Position.x - TowerAI.Radius;
                var right = Position.x + TowerAI.Radius;
                var top = Position.y + TowerAI.Radius;
                var bottom = Position.y - TowerAI.Radius;

                float distance = TowerAI.Radius;
                UnitModel selectedUnit = null;
                foreach (var unit in _map.Units)
                {
                    if (unit.Position.x >= left && unit.Position.y <= right && unit.Position.y >= bottom && unit.Position.y <= top)
                    {
                        var dist = Position.DistanceTo(unit.Position);
                        if (dist <= distance)
                        {
                            distance = dist;
                            selectedUnit = unit;
                        }
                    }
                }
                if (selectedUnit != null)
                    _target = selectedUnit;
            }
            Direction = _target == null ? Vector2.up : (_target.Position - Position).normalized;
        }
    }
}