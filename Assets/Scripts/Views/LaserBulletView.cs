using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;
using Managers;

namespace Views
{
    public class LaserBulletView : BulletView
    {
        [SerializeField]
        private LineRenderer _lineRenderer;

        private LaserBulletModel _laserBullet;

        protected override void Update()
        {
            if (_laserBullet == null)
                _laserBullet = _bullet as LaserBulletModel;

            transform.localPosition = GraphicsManager.Scale(_bullet.Position);
            if (_laserBullet.Target != null)
            {
                _lineRenderer.enabled = true;
                _lineRenderer.SetPosition(1, GraphicsManager.Scale(_laserBullet.Target.Position - _bullet.Position));
            }
            else
            {
                _lineRenderer.enabled = false;
            }
        }
    }
}
