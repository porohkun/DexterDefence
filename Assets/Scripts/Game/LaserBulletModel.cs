using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class LaserBulletModel:BulletModel
    {
        public UnitModel Target { get { return _target; } }

        public LaserBulletModel(string visual, Vector2 position, UnitModel target, float speed):base(visual,position,target,speed)
        {
            Position = position;
        }

        public void SetTarget(UnitModel target)
        {
            _target = target;
        }

        public override void Update(float deltaTime)
        {
            
        }

        public void ManualHit()
        {
            OnHitted();
        }
    }
}
