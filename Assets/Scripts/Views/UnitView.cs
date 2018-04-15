using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;
using Managers;

namespace Views
{
    public class UnitView : MonoBehaviour
    {
        private UnitModel _unit;

        public void AttachTo(UnitModel unit)
        {
            _unit = unit;
            Update();
        }

        private void Update()
        {
            transform.localPosition = GraphicsManager.Scale(_unit.Position);
            transform.rotation = Quaternion.LookRotation(Vector3.forward, _unit.Direction);
        }
    }
}
