using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;
using Managers;

namespace Views
{
    public class BulletView : MonoBehaviour
    {
        private BulletModel _bullet;

        public void AttachTo(BulletModel bullet)
        {
            _bullet = bullet;
            Update();
        }

        private void Update()
        {
            transform.localPosition = GraphicsManager.Scale(_bullet.Position);
            transform.rotation = Quaternion.LookRotation(Vector3.forward, _bullet.Direction);
        }
    }
}
