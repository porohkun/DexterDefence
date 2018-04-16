using System;
using UnityEngine;

namespace Game
{
    public class TowerModel
    {
        public Shell ShellType { get; private set; }
        public ITowerAI TowerAI { get; private set; }
        public float RotateSpeed { get; private set; }
        public Point Position { get; private set; }

        private MapModel _map;

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

        }
    }
}