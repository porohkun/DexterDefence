using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;
using Managers;

namespace Views
{
    public class TowerView : MonoBehaviour
    {
        [SerializeField]
        private Transform _turret;
        [SerializeField]
        private TextMesh _level;
        private TowerModel _tower;

        public int Level { get { return _tower.Level; } }
        public bool CanBeUpgraded { get { return _tower.CanBeUpgraded; } }
        public int UpgradeCost { get { return _tower.UpgradeCost; } }

        public void AttachTo(TowerModel tower)
        {
            _tower = tower;
            transform.localPosition = GraphicsManager.Scale(_tower.Position);
            Update();
        }

        private void Update()
        {
            _turret.rotation = Quaternion.LookRotation(Vector3.forward, _tower.Direction);
            _level.text = (Level + 1).ToString();
        }

        public void UpgradeTower()
        {
            _tower.UpgradeTower();
        }
    }
}
