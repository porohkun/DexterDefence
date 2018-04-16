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
        public event Action<BulletModel> BulletShoot;
        public int Level { get { return TowerAI.Level; } }
        public bool CanBeUpgraded { get { return TowerAI.CanBeUpgraded; } }
        public int UpgradeCost { get { return TowerAI.UpgradeCost; } }
        public int Cashback { get { return TowerAI.Cashback; } }

        private MapModel _map;
        private UnitModel _target;

        public TowerModel(Shell shellType, ITowerAI towerAI, float rotateSpeed)
        {
            ShellType = shellType;
            TowerAI = towerAI;
            TowerAI.BulletShoot += TowerAI_BulletShoot;
            RotateSpeed = rotateSpeed;
        }

        public void UpgradeTower()
        {
            TowerAI.UpgradeTower();
        }

        private void TowerAI_BulletShoot(BulletModel bullet)
        {
            if (BulletShoot != null)
                BulletShoot(bullet);
        }

        public void Initialize(MapModel map, Point position)
        {
            _map = map;
            Position = position;
            TowerAI.Initialize(Position);
        }

        public void Update(float deltaTime)
        {
            if (_target != null)
                if (Position.DistanceTo(_target.Position) > TowerAI.Radius)
                    ClearTarget();

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
                    SetTarget(selectedUnit);
            }

            Direction = _target == null ? Vector2.up : (_target.Position - Position).normalized;

            if (_target != null)
                TowerAI.Update(deltaTime);
        }

        private void ClearTarget()
        {
            _target.Died -= _target_Invalid;
            _target.Finished -= _target_Invalid;
            _target = null;
            TowerAI.ClearTarget();
        }

        private void _target_Invalid(UnitModel obj)
        {
            ClearTarget();
        }

        private void SetTarget(UnitModel selectedUnit)
        {
            _target = selectedUnit;
            _target.Died += _target_Invalid;
            _target.Finished += _target_Invalid;
            TowerAI.SetTarget(_target);
        }
    }
}