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
        private TowerModel _tower;

        public void AttachTo(TowerModel tower)
        {
            _tower = tower;
            transform.localPosition = GraphicsManager.Scale(_tower.Position);
            Update();
        }

        private void Update()
        {
            _turret.rotation = Quaternion.LookRotation(Vector3.forward, _tower.Direction);
        }
    }
}
