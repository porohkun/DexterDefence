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
        private TowerModel _tower;

        public void AttachTo(TowerModel tower)
        {
            _tower = tower;
            transform.localPosition = GraphicsManager.Scale(_tower.Position);
            Update();
        }

        private void Update()
        {
            //transform.rotation = Quaternion.LookRotation(Vector3.forward, _unit.Direction);
        }
    }
}
